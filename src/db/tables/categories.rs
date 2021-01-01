use super::get_conn;
use crate::{
  db::{Connection, Pool},
  models::{Category, ID},
};
use sqlx::{Error, Executor};

pub struct Categories {
  pool: Pool,
}

const INIT: &str = "
  CREATE TABLE IF NOT EXISTS modmail.categories (
    id BIGINT NOT NULL
      CONSTRAINT categories_pk PRIMARY KEY,
    channel_id BIGINT UNIQUE NOT NULL,
    name TEXT NOT NULL,
    is_active BOOLEAN DEFAULT true NOT NULL,
    guild_id BIGINT NOT NULL,
    emote TEXT NOT NULL),";

impl Categories {
  pub async fn init(pool: Pool) -> Result<Self, Error> {
    let mut conn: Connection = get_conn(&pool).await?;

    conn.execute(INIT).await?;

    Ok(Self { pool })
  }

  pub async fn get_active(&self) -> Vec<Category> {
    todo!("implement");
  }

  pub async fn get_all(&self) -> Vec<Category> {
    todo!("implement");
  }

  pub async fn get_by_emote(&self, emoji: String) -> Option<Category> {
    todo!("implement");
  }

  pub async fn get_by_name(&self, name: String) -> Option<Category> {
    todo!("implement");
  }

  pub async fn get_by_id(&self, category_id: ID) -> Option<Category> {
    todo!("implement");
  }

  pub async fn set_active(&self, category_id: ID, active: bool) -> Result<bool, sqlx::Error> {
    todo!("implement");
  }

  pub async fn set_emote(&self, category_id: ID, emoji: String) -> Result<bool, sqlx::Error> {
    todo!("implement");
  }

  pub async fn set_name(&self, category_id: ID, name: String) -> Result<bool, sqlx::Error> {
    todo!("implement");
  }
}
