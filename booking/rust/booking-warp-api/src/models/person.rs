use chrono::{DateTime, Utc};
use uuid::Uuid;

pub struct Person {
    id: Uuid,
    first_name: String,
    last_name: String,
    email: String,
    phone: String,
    created: DateTime<Utc>,
    updated: DateTime<Utc>,
    deleted: Option<DateTime<Utc>>,
}
