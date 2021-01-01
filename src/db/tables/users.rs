use super::get_conn;
use crate::db::{Connection, Pool};
use sqlx::{Error, Executor};

pub struct Users {
  pool: Pool,
}

const INIT: &str = "
  CREATE TABLE IF NOT EXISTS modmail.users (
    id BIGINT NOT NULL
      CONSTRAINT users_pk PRIMARY KEY);

  CREATE UNIQUE INDEX IF NOT EXISTS users_id_uindex
    ON modmail.users (id);";

impl Users {
  pub async fn init(pool: Pool) -> Result<Self, Error> {
    let mut conn: Connection = get_conn(&pool).await?;

    conn.execute(INIT).await?;

    Ok(Self { pool })
  }
}
