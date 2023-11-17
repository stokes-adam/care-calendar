use jsonwebtoken::*;
use crate::models::{
    app_state::{
        AppState,
        State
    },
    claims::Claims,
    auth_config::AuthConfig,
    auth::{
        TokenResponse,
        TokenRequest,
        Jwks,
        Jwk,
        AuthError
    },
};

const GRANT_TYPE: &str = "authorization_code";

pub async fn exchange_code_for_tokens(code: &str, auth_config: &AuthConfig) -> Result<TokenResponse, String> {
    let request = TokenRequest {
        client_id: auth_config.client_id.clone(),
        client_secret: auth_config.client_secret.clone(),
        code: code.to_string(),
        grant_type: GRANT_TYPE.to_string(),
        redirect_uri: auth_config.redirect_uri.clone()
    };

    let url = format!("https://{}/oauth/token", &auth_config.client_domain);

    let response = reqwest::Client::new()
        .post(&url)
        .json(&request)
        .send()
        .await
        .or_else(|_| Err("Failed to send request".to_string()))?;

    response
        .json::<TokenResponse>()
        .await
        .or_else(|_| Err("Failed to parse response".to_string()))
}

pub async fn do_auth(cookie: Option<String>, shared_state: State) -> Result<Claims, AuthError> {
    let token = cookie
        .ok_or_else(|| {
            AuthError{ message: "No cookie".to_string() }
        })?;

    let decoded_header = decode_header(&token)
        .or_else(|_| Err(AuthError{ message: "No header".to_string() }))?;

    let kid = decoded_header.kid
        .ok_or_else(|| AuthError{ message: "No kid".to_string() })?;

    let mut app_state = shared_state.lock().await;

    let public_key = find_public_key(&kid, &mut app_state).await
        .or_else(|_| Err(AuthError{ message: "Failed to find public key".to_string() }))?;

    let decoding_key = DecodingKey::from_rsa_components(&public_key.n, &public_key.e)
        .or_else(|_| Err(AuthError{ message: "Failed to create decoding key".to_string() }))?;

    let mut validation = Validation::new(Algorithm::RS256);

    validation.set_audience(&[app_state.auth_config.client_id.as_str()]);

    let token_data = decode::<Claims>(
        &token,
        &decoding_key,
        &validation,
    )
        .or_else(|_| Err(AuthError{ message: "Failed to decode token".to_string() }))?;

    Ok(token_data.claims)
}

async fn find_public_key(kid: &str, shared_state: &mut AppState) -> Result<Jwk, AuthError> {
    let local_key = shared_state.jwks.keys
        .iter()
        .find(|jwk| jwk.kid == kid)
        .map(|jwk| jwk.clone());

    match local_key {
        Some(key) => Ok(key),
        None => {
            let response = fetch_kids(&shared_state.auth_config.client_domain)
                .await
                .or_else(|err| Err(AuthError{ message: format!("Failed to fetch kids: {}", err) }))?;

            let key = response.keys
                .iter()
                .find(|key| key.kid == kid)
                .map(|key| key.clone())
                .unwrap();

            shared_state.jwks.keys.push(key.clone());

            Ok(key)
        }
    }
}

async fn fetch_kids(client_domain: &String) -> Result<Jwks, reqwest::Error> {
    reqwest::get(format!("https://{}/.well-known/jwks.json", &client_domain))
        .await?
        .json::<Jwks>()
        .await
}
