use crate::{
  db::Threads as ThdTable,
};


pub struct Threads {
  pub db: ThdTable,
}

impl Threads {
  pub fn new(table: ThdTable) -> Self {
    Self {
      db: table,
    }
  }
}
