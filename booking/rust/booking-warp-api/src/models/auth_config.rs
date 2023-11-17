use serde::Deserialize;

#[derive(Debug, Deserialize)]
pub struct AuthConfig {
    pub client_domain: String,
    pub client_id: String,
    pub client_secret: String,
    pub redirect_uri: String,
}

impl AuthConfig {
    pub fn from_file(file_name: &str) -> Result<AuthConfig, String> {
        let contents = std::fs::read_to_string(file_name)
            .map_err(|e| e.to_string())?;

        serde_json::from_str(&contents)
            .map_err(|e| e.to_string())
    }
}
