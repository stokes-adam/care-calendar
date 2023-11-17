use serde::{Serialize, Deserialize};

#[derive(Debug, Deserialize)]
pub struct Jwks {
    pub keys: Vec<Jwk>
}

impl Jwks {
    pub fn new() -> Jwks {
        Jwks {
            keys: Vec::new()
        }
    }
}

#[derive(Debug, Clone, Deserialize)]
pub struct Jwk {
    pub n: String,
    pub e: String,
    pub kid: String,
}

#[derive(Serialize, Deserialize)]
pub struct TokenRequest {
    pub client_id: String,
    pub client_secret: String,
    pub code: String,
    pub grant_type: String,
    pub redirect_uri: String,
}

#[derive(Serialize, Deserialize)]
pub struct TokenResponse {
    pub access_token: String,
    pub id_token: String,
    pub expires_in: u64,
    pub token_type: String,
}

#[derive(Debug)]
pub struct AuthError {
    pub message: String
}

impl warp::reject::Reject for AuthError {}
