mod commands;

use crate::{
  config::BotConfig,
  controllers::Controllers,
};
use log::{error};
use serenity::{
  Client,
  framework::{
    StandardFramework,
  }
};

struct Bot {
  ctrl: Controllers,
}


pub async fn start(config: BotConfig, ctrl: Controllers) {
  let frmwrk = get_framework(config.prefix);
  let mut client = Client::builder(config.token)
    .framework(frmwrk)
    .await
    .unwrap();

  if let Err(why) = client.start().await {
    error!("Failed to connect to Discord");
    panic!(why);
  }
}

fn get_framework(prefix: String) -> StandardFramework {
  let framework = StandardFramework::new()
    .configure(|c| {
      c.prefix(&prefix);
      return c;
    });

  return framework;
}
