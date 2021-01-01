use super::get_conn;
use crate::db::{Connection, Pool};
use sqlx::{Error, Executor};

pub struct StandardReplies {
  pool: Pool,
}

const INIT: &str = "
  CREATE TABLE IF NOT EXISTS modmail.standard_replies (
    id BIGINT NOT NULL
      CONSTRAINT standard_replies_pk PRIMARY KEY,
    name TEXT NOT NULL,
    reply TEXT NOT NULL);

  CREATE UNIQUE INDEX IF NOT EXISTS standard_replies_id_uindex
    ON modmail.standard_replies (id);

  CREATE UNIQUE INDEX IF NOT EXISTS standard_replies_name_uindex
    ON modmail.standard_replies (name);";

impl StandardReplies {
  pub async fn init(pool: Pool) -> Result<Self, Error> {
    let mut conn: Connection = get_conn(&pool).await?;

    conn.execute(INIT).await?;

    Ok(Self { pool })
  }
}
