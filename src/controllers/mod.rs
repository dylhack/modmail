mod attachments;
mod categories;
mod messages;
mod threads;
use self::{
  attachments::Attachments as AttCtrl, categories::Categories as CatCtrl,
  messages::Messages as MsgCtrl, threads::Threads as ThdCtrl,
};
use crate::db::DBClient;

pub struct Controllers {
  pub attachments: AttCtrl,
  pub categories: CatCtrl,
  pub messages: MsgCtrl,
  pub threads: ThdCtrl,
}

impl Controllers {
  pub fn new(db: DBClient) -> Self {
    Self {
      attachments: AttCtrl::new(db.attachments),
      categories: CatCtrl::new(db.categories),
      messages: MsgCtrl::new(db.messages),
      threads: ThdCtrl::new(db.threads),
    }
  }
}
