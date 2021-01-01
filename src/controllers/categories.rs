use crate::{
  db::Categories as CatTable,
};

pub struct Categories {
  pub db: CatTable,
}

impl Categories {
  pub fn new(table: CatTable) -> Self {
    Self {
      db: table,
    }
  }
}
