use std::sync::Arc;
use futures::lock::Mutex;

use crate::models::app_state::State;
use crate::models::app_state::AppState;
use crate::models::auth_config::AuthConfig;
use crate::models::auth::Jwks;
use crate::services::encryptor::Encryptor;

const DB_VAR: &str = "DATABASE_URL";

pub async fn init() -> State {
    let auth_config = AuthConfig::from_file("oidc.json")
        .expect("Failed to load oidc.json");

    let pool = create_db_pool()
        .await
        .expect("Failed to create db pool");

    let encryptor = Encryptor::new(None)
        .await;

    let app_state = AppState {
        auth_config,
        jwks: Jwks::new(),
        pool,
        encryptor
    };

    Arc::new(Mutex::new(app_state))
}

async fn create_db_pool() -> Result<sqlx::PgPool, String> {
    let url = std::env::var(DB_VAR)
        .map_err(|_| format!("{} must be set", DB_VAR))?;

    sqlx::pool::PoolOptions::new()
        .max_connections(5)
        .connect(&url)
        .await
        .map_err(|_| "Unable to connect to database".to_string())
}
