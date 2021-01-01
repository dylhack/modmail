use super::get_conn;
use crate::{
  db::{Connection, Pool},
  models::{Edit, ID},
};
use sqlx::{Error, Executor};

pub struct Edits {
  pool: Pool,
}

const INIT: &str = "
  CREATE TABLE IF NOT EXISTS modmail.edits (
    content TEXT NOT NULL,
    message BIGINT NOT NULL
      CONSTRAINT edits_messages_modmail_id_fk
      REFERENCES modmail.messages (modmail_id),
    version INTEGER DEFAULT 1 NOT NULL)";

impl Edits {
  pub async fn init(pool: Pool) -> Result<Self, Error> {
    let mut conn: Connection = get_conn(&pool).await?;

    conn.execute(INIT).await?;

    Ok(Self { pool })
  }

  pub async fn store(&self, edit: &Edit) -> Result<bool, sqlx::Error> {
    todo!("implement");
  }

  pub async fn get_edits(message_id: ID) -> Result<Vec<Edit>, sqlx::Error> {
    todo!("implement");
  }
}
