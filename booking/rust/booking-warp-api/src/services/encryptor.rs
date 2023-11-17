use aws_sdk_kms::config::Region;
use aws_sdk_kms::Client;

#[derive(Debug, Clone)]
enum EncryptorType {
    NoEncryption,
    AwsEncryption(Client, String),
}

#[derive(Debug, Clone)]
pub struct Encryptor {
    encryption_type: EncryptorType,
}

pub struct EncryptorOptions {
    key_id: String,
    region: String,
}

impl Encryptor {
    pub async fn new(options: Option<EncryptorOptions>) -> Self {
        match options {
            Some(options) => {
                let EncryptorOptions { key_id, region } = options;
                let region = Region::new(region);

                let shared_config = aws_config::from_env()
                    .region(region)
                    .load()
                    .await;

                let kms_client = Client::new(&shared_config);

                Encryptor {
                    encryption_type: EncryptorType::AwsEncryption(kms_client, key_id),
                }
            },
            None => Encryptor {
                encryption_type: EncryptorType::NoEncryption,
            },
        }
    }

    pub async fn encrypt(&self, data: &str) -> Option<String> {
        match &self.encryption_type {
            EncryptorType::NoEncryption => Some(data.to_string()),
            _ => None,
        }
    }

    pub async fn decrypt(&self, data: &str) -> Option<String> {
        match &self.encryption_type {
            EncryptorType::NoEncryption => Some(data.to_string()),
            _ => None,
        }
    }
}

