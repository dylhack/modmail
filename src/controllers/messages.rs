use crate::{
  db::Messages as MsgTable,
};

pub struct Messages {
  pub db: MsgTable,
}

impl Messages {
  pub fn new(table: MsgTable) -> Self {
    Self {
      db: table,
    }
  }
}
