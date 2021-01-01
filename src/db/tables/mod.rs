mod attachments;
mod categories;
mod edits;
mod messages;
mod mutes;
mod permissions;
mod standard_replies;
mod threads;
mod users;

pub use attachments::Attachments;
pub use categories::Categories;
pub use edits::Edits;
pub use messages::Messages;
pub use mutes::Mutes;
pub use permissions::Permissions;
pub use standard_replies::StandardReplies;
pub use threads::Threads;
pub use users::Users;

use crate::db::{Connection, Pool};
use log::error;
use sqlx::Error;

async fn get_conn(pool: &Pool) -> Result<Connection, Error> {
  match pool.acquire().await {
    Ok(conn) => Ok(conn),
    Err(e) => {
      error!("Failed to acquire connection");
      Err(e)
    }
  }
}
