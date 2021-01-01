use super::get_conn;
use crate::{
  db::{Connection, Pool},
  models::Attachment,
};
use sqlx::{Error, Executor};

pub struct Attachments {
  pool: Pool,
}

const INIT: &str = "
  CREATE TABLE IF NOT EXISTS modmail.attachments (
    id BIGINT NOT NULL
      CONSTRAINT attachments_pk PRIMARY KEY,
    message_id BIGINT NOT NULL
      CONSTRAINT attachments_messages_modmail_id_fk
      REFERENCES modmail.messages (modmail_id),
    name TEXT NOT NULL,
    source TEXT NOT NULL,
    sender BIGINT NOT NULL
      CONSTRAINT attachments_users_id_fk
      REFERENCES modmail.users,
    file_type modmail.file_type DEFAULT 'file'::modmail.file_type NOT NULL);

  CREATE UNIQUE INDEX IF NOT EXISTS categories_emote_uindex
    ON modmail.attachments (emote);

  CREATE UNIQUE INDEX IF NOT EXISTS categories_id_uindex
    ON modmail.attachments (id);

  CREATE UNIQUE INDEX IF NOT EXISTS categories_name_uindex
    ON modmail.attachments (name);";

impl Attachments {
  pub async fn init(pool: Pool) -> Result<Self, Error> {
    let mut conn: Connection = get_conn(&pool).await?;

    conn.execute(INIT).await?;

    return Ok(Self { pool });
  }

  pub async fn store(attachment: &Attachment) {
    todo!("implement");
  }
}
