use std::collections::HashMap;
use std::convert::Infallible;
use crate::models::app_state::State;
use crate::models::auth::AuthError;
use crate::services::auth::exchange_code_for_tokens;

pub async fn build_authorize_url(shared_state: State) -> Result<String, Infallible> {
    let app_state = shared_state.lock().await;
    let config = &app_state.auth_config;
    let url = format!(
            "https://{}/authorize?response_type=code&client_id={}&redirect_uri={}&scope=openid profile email",
            config.client_domain,
            config.client_id,
            config.redirect_uri
        );

    Ok(url)
}

pub async fn generate_cookie_handler(app_state: State, query: HashMap<String, String>) -> Result<String, warp::Rejection> {
    gen_cookie(app_state, query)
        .await
        .map_err(warp::reject::custom)
}

pub async fn gen_cookie(app_state: State, query: HashMap<String, String>) -> Result<String, AuthError> {
    let shared = app_state.lock().await;

    let code = query.get("code")
        .ok_or(AuthError { message: "No code in query".to_string() })?;

    let tokens = exchange_code_for_tokens(&code, &shared.auth_config)
        .await
        .map_err(|e| AuthError { message: format!("Error exchanging code for tokens: {}", e) })?;

    Ok(format!("session={}; Path=/; HttpOnly", tokens.id_token))
}

