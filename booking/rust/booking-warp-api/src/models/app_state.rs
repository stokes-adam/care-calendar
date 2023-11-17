use crate::models::auth::Jwks;
use crate::models::auth_config::AuthConfig;
use crate::services::encryptor::Encryptor;

#[derive(Debug)]
pub struct AppState {
    pub auth_config: AuthConfig,
    pub jwks: Jwks,
    pub pool: sqlx::PgPool,
    pub encryptor: Encryptor,
}

pub type State = std::sync::Arc<futures::lock::Mutex<AppState>>;
