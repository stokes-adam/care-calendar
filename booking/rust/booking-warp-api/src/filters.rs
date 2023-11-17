use std::convert::Infallible;
use warp::{
    any,
    Filter,
};
use crate::models::{
    app_state::State,
    claims::Claims,
};
use crate::services::auth::do_auth;

const SESSION_COOKIE: &str = "session";

pub fn state_filter(state: &State) -> impl Filter<Extract = (State,), Error = Infallible> + Clone {
    let state = state.clone();
    any().map(move || state.clone())
}

pub fn auth_filter(state: &State) -> impl Filter<Extract = (Claims,), Error = warp::Rejection> + Clone {
    any()
        .and(warp::cookie::optional(SESSION_COOKIE))
        .and(state_filter(&state))
        .and_then(|cookie, state| async move {
            do_auth(cookie, state).await
                .map_err(warp::reject::custom)
        })
}
