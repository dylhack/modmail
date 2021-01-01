use log::error;
use serde::{Deserialize, Serialize};
use std::{env::var, io, path::Path};
use tokio::{
  fs::File,
  io::{AsyncReadExt, AsyncWriteExt},
};

const CONFIG_PATH: &str = "config.yml";
const CONFIG_PATH_ENV: &str = "MODMAIL_CONFIG";
pub const LOG_CONFIG_PATH: &str = "log4rs.yaml";

#[derive(Serialize, Deserialize)]
pub struct BotConfig {
  pub token: String,
  pub prefix: String,
}

#[derive(Serialize, Deserialize)]
pub struct DBConfig {
  pub address: String,
  pub port: u16,
  pub username: String,
  pub password: String,
  pub database: String,
  pub max_connections: u8,
}

#[derive(Serialize, Deserialize)]
pub struct Config {
  pub bot: BotConfig,
  pub database: DBConfig,
}

impl BotConfig {
  pub fn new() -> BotConfig {
    BotConfig {
      token: String::from(""),
      prefix: String::from("!"),
    }
  }
}

impl DBConfig {
  pub fn new() -> DBConfig {
    DBConfig {
      address: String::from("127.0.0.1"),
      port: 5432,
      username: String::from("postgres"),
      password: String::from("password"),
      database: String::from("postgres"),
      max_connections: 200,
    }
  }
}

impl Config {
  pub async fn get_config() -> Config {
    let target = Self::get_path();
    let path = Path::new(target.as_str());

    if path.exists() {
      return Self::load(&target).await;
    }
    return Self::new().await;
  }

  pub async fn save(&self) {
    let path = Self::get_path();
    let serialized = serde_yaml::to_vec(&self).unwrap();
    let mut file = Self::get_file(false, &path).await;

    if let Err(why) = file.write_all(&serialized).await {
      error!("Failed to save config {}", path);
      panic!(why);
    }
  }

  async fn get_file(create: bool, path: &String) -> File {
    let file_res: io::Result<File>;
    if create {
      file_res = File::create(path).await;
    } else {
      file_res = File::open(path).await;
    }

    match file_res {
      Ok(file) => file,
      Err(e) => {
        if create {
          error!("Failed to create config file at {}", path);
        } else {
          error!("Failed to open config file at {}", path);
        }
        panic!(e);
      }
    }
  }

  fn get_path() -> String {
    match var(CONFIG_PATH_ENV) {
      Ok(path) => path,
      Err(_) => String::from(CONFIG_PATH),
    }
  }

  async fn load(loc: &String) -> Config {
    let mut file = Self::get_file(false, loc).await;
    let mut contents = vec![];

    if let Err(why) = file.read_to_end(&mut contents).await {
      error!("Failed to deserialize config from {}", why);
      panic!(why);
    }

    match serde_yaml::from_slice(&mut contents) {
      Ok(config) => config,
      Err(e) => {
        error!("Failed to deserialize config from {}", loc);
        panic!(e);
      }
    }
  }

  async fn new() -> Config {
    let config = Config {
      bot: BotConfig::new(),
      database: DBConfig::new(),
    };

    config.save().await;

    return config;
  }
}
