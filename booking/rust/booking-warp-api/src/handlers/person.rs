use uuid::Uuid;
use warp::reply::json;
use warp::reply::Json;

use crate::models::person::Person;
use crate::models::claims::Claims;

pub fn get_person(id: Uuid, claims: Claims) -> Json {
    json(&claims)
}
