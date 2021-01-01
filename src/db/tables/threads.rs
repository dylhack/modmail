use super::get_conn;
use crate::db::{Connection, Pool};
use sqlx::{Error, Executor};

pub struct Threads {
  pool: Pool,
}

const INIT: &str = "
  CREATE TABLE IF NOT EXISTS modmail.threads (
    id BIGINT NOT NULL
      CONSTRAINT threads_pk PRIMARY KEY,
    author BIGINT NOT NULL
      CONSTRAINT threads_users_id_fk
      REFERENCES modmail.users,
    channel BIGINT NOT NULL,
    is_active BOOLEAN DEFAULT true NOT NULL,
    category BIGINT NOT NULL
      CONSTRAINT threads_categories_id_fk
      REFERENCES modmail.categories);

  CREATE UNIQUE INDEX IF NOT EXISTS threads_channel_uindex
    ON modmail.threads (channel);

  CREATE UNIQUE INDEX IF NOT EXISTS threads_channel_uindex_2
    ON modmail.threads (channel);

  CREATE UNIQUE INDEX IF NOT EXISTS threads_id_uindex
    ON modmail.threads (id);";

impl Threads {
  pub async fn init(pool: Pool) -> Result<Self, Error> {
    let mut conn: Connection = get_conn(&pool).await?;

    conn.execute(INIT).await?;

    Ok(Self { pool })
  }
}
