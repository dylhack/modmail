use crate::db::Attachments as AttachTable;

pub struct Attachments {
  pub db: AttachTable,
}

impl Attachments {
  pub fn new(table: AttachTable) -> Self {
    Self { db: table }
  }
}
