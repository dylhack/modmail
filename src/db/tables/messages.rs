use super::get_conn;
use crate::{
  db::{Connection, Pool},
  models::{Message, ID},
};
use sqlx::{Error, Executor};

pub struct Messages {
  pool: Pool,
}

const INIT: &str = "
  CREATE TABLE IF NOT EXISTS modmail.messages (
    sender BIGINT NOT NULL
      CONSTRAINT messages_users_id_fk
      REFERENCES modmail.users,
    client_id BIGINT,
    modmail_id BIGINT NOT NULL,
    content TEXT NOT NULL,
    thread_id BIGINT NOT NULL
      CONSTRAINT messages_threads_id_fk
      REFERENCES modmail.threads,
    is_deleted BOOLEAN DEFAULT false NOT NULL,
    internal BOOLEAN DEFAULT false NOT NULL);

    CREATE UNIQUE INDEX IF NOT EXISTS messages_client_id_uindex
      ON modmail.messages (client_id);

    CREATE UNIQUE INDEX IF NOT EXISTS messages_modmail_id_uindex
      ON modmail.messages (modmail_id);";

impl Messages {
  pub async fn init(pool: Pool) -> Result<Self, Error> {
    let mut conn: Connection = get_conn(&pool).await?;

    conn.execute(INIT).await?;

    Ok(Self { pool })
  }

  pub async fn store(&self, message: &Message) -> Result<bool, sqlx::Error> {
    todo!("implement");
  }

  pub async fn get_last_message(&self, channel_id: &ID) -> Result<Option<Message>, sqlx::Error> {
    todo!("implement");
  }

  pub async fn get_messages(&self, channel_id: &ID) -> Result<Vec<Message>, sqlx::Error> {
    todo!("implement");
  }

  pub async fn set_deleted(&self, message_id: &ID) -> Result<bool, sqlx::Error> {
    todo!("implement");
  }
}
