use serde::{Serialize, Deserialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct Claims {
    pub sub: String,
    pub given_name: String,
    pub family_name: String,
    pub nickname: String,
    pub name: String,
    pub picture: String,
    pub locale: String,
    pub updated_at: String,
    pub email: String,
    pub email_verified: bool,
}
