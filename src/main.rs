mod bot;
mod config;
mod controllers;
mod db;
mod models;

use self::{
  config::Config,
  controllers::Controllers,
};
use log::info;

#[tokio::main]
async fn main() {
    start_logger();

    let config = Config::get_config().await;
    let db_client = db::start(config.database).await;
    let controllers = Controllers::new(db_client);

    bot::start(config.bot, controllers).await;
}

fn start_logger() {
    let init_res = log4rs::init_file(config::LOG_CONFIG_PATH, Default::default());

    match init_res {
        Ok(_) => info!("Logger ready."),
        Err(e) => {
            println!("Failed to start log4rs");
            panic!(e);
        }
    };
}
