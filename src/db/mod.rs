mod tables;
use crate::config::DBConfig;
use log::error;
use sqlx::{
  pool::PoolConnection,
  postgres::{PgPool, PgPoolOptions},
  Postgres,
};
pub use tables::{
  Attachments, Categories, Edits, Messages, Mutes, Permissions, StandardReplies, Threads, Users,
};

pub type Pool = PgPool;
pub type Connection = PoolConnection<Postgres>;
pub struct DBClient {
  pub attachments: Attachments,
  pub categories: Categories,
  pub edits: Edits,
  pub messages: Messages,
  pub mutes: Mutes,
  pub perms: Permissions,
  pub standard_replies: StandardReplies,
  pub threads: Threads,
  pub users: Users,
}

const FAILED_INIT: &str = "Failed to initialize table.";

impl DBClient {
  pub async fn new(pool: Pool) -> DBClient {
    let attachments = Attachments::init(pool.clone()).await.expect(FAILED_INIT);
    let categories = Categories::init(pool.clone()).await.expect(FAILED_INIT);
    let edits = Edits::init(pool.clone()).await.expect(FAILED_INIT);
    let messages = Messages::init(pool.clone()).await.expect(FAILED_INIT);
    let mutes = Mutes::init(pool.clone()).await.expect(FAILED_INIT);
    let perms = Permissions::init(pool.clone()).await.expect(FAILED_INIT);
    let sr = StandardReplies::init(pool.clone())
      .await
      .expect(FAILED_INIT);
    let threads = Threads::init(pool.clone()).await.expect(FAILED_INIT);
    let users = Users::init(pool.clone()).await.expect(FAILED_INIT);

    DBClient {
      attachments,
      categories,
      edits,
      messages,
      mutes,
      perms,
      standard_replies: sr,
      threads,
      users,
    }
  }
}

pub async fn start(config: DBConfig) -> DBClient {
  let pool_res = PgPoolOptions::new()
    .max_connections(5)
    .connect(
      format!(
        "postgres://{}:{}@{}:{}/{}",
        config.username, config.password, config.address, config.port, config.database,
      )
      .as_str(),
    )
    .await;

  match pool_res {
    Ok(pool) => DBClient::new(pool).await,
    Err(why) => {
      error!(
        "Failed to connect to postgres database {}:{}",
        config.address, config.port,
      );
      panic!(why);
    }
  }
}
