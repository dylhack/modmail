use super::get_conn;
use crate::db::{Connection, Pool};
use sqlx::{Error, Executor};

pub struct Permissions {
  pool: Pool,
}

const INIT: &str = "
  CREATE TABLE IF NOT EXISTS modmail.permissions (
    category_id BIGINT NOT NULL
      REFERENCES modmail.categories,
    role_id TEXT UNIQUE NOT NULL,
    level modmail.role_level DEFAULT 'mod'::modmail.role_level NOT NULL);";

impl Permissions {
  pub async fn init(pool: Pool) -> Result<Self, Error> {
    let mut conn: Connection = get_conn(&pool).await?;

    conn.execute(INIT).await?;

    Ok(Self { pool })
  }
}
