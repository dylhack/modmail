use super::get_conn;
use crate::{
  db::{Connection, Pool},
  models::{MuteStatus, ID},
};
use sqlx::{Error, Executor};

pub struct Mutes {
  pool: Pool,
}

const INIT: &str = "
  CREATE TABLE IF NOT EXISTS modmail.mutes (
    user_id BIGINT NOT NULL
      CONSTRAINT threads_users_id_fk
      REFERENCES modmail.users,
    till BIGINT NOT NULL,
      category_id BIGINT NOT NULL,
    reason TEXT NOT NULL);";

impl Mutes {
  pub async fn init(pool: Pool) -> Result<Self, Error> {
    let mut conn: Connection = get_conn(&pool).await?;

    conn.execute(INIT).await?;

    Ok(Self { pool })
  }

  pub async fn store(&self, mute: &MuteStatus) -> Result<bool, sqlx::Error> {
    todo!("implement");
  }

  pub async fn revoke(&self, user_id: &ID, category_id: &ID) -> Result<bool, sqlx::Error> {
    todo!("implement");
  }
}
