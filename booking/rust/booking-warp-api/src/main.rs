mod filters;
mod handlers;
mod models;
mod state;
mod services;

use std::collections::HashMap;
use uuid::Uuid;
use warp::{
    serve,
    get,
    path,
    query,
    reply::json,
    http::Response,
    Filter,
};

type QueryMap = HashMap<String, String>;

#[tokio::main]
async fn main() {
    dotenv::dotenv().ok();

    let state = state::init().await;

    let ping = get()
        .and(path!("ping"))
        .map(|| "pong");

    let auth_test = get()
        .and(path!("auth" / "test"))
        .and(filters::auth_filter(&state))
        .map(|claims| {
            json(&claims)
        });
    
    let callback = get()
        .and(path!("callback"))
        .and(filters::state_filter(&state))
        .and(query::<QueryMap>())
        .and_then(handlers::auth::generate_cookie_handler)
        .map(|cookie| Response::builder()
            .status(warp::http::StatusCode::FOUND)
            .header("Set-Cookie", cookie)
            .header("Location", "/home")
            .body("done".to_string())
        );

    let authorize = get()
        .and(path!("authorize"))
        .and(filters::state_filter(&state))
        .and_then(handlers::auth::build_authorize_url)
        .map(|url| Response::builder()
            .status(warp::http::StatusCode::FOUND)
            .header("Location", url)
            .body("done".to_string())
        );

    let get_person = get()
        .and(path!("person" / Uuid))
        .and(filters::auth_filter(&state))
        .map(handlers::person::get_person);

    let routes = ping
        .or(auth_test)
        .or(callback)
        .or(authorize)
        .or(get_person);

    serve(routes)
        .run(([0, 0, 0, 0], 5057))
        .await;
}
