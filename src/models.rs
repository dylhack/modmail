pub type ID = u64;

// An attachment of a message
pub struct Attachment {
  // The message that this attachment came from
  pub message_id: ID,
  // File name
  pub name: String,
  // Discord user that sent it
  pub sender: ID,
  // Discord CDN link
  pub source: String,
  // Whether it's an image or not
  pub is_image: bool,
}

// A Category represents a Guild
pub struct Category {
  // Human-readable name
  pub name: String,
  // Emoji that represents this Category
  pub emoji: String,
  // Guild that this Category represents
  pub guild: ID,
  // ID that represents this Category
  pub id: ID,
  // Whether or not threads can be made in this Category
  pub is_active: bool,
  // The Category **CHANNEL** where new thread channels will be made
  pub channel_id: ID,
}

pub struct Edit {
  // New message content
  pub content: String,
  // The message edited
  pub message_id: ID,
  // A simple increment of versions. The first edit is 1
  pub version: u32,
}

pub struct Message {
  // Human readable contents of the message
  pub content: String,
  // The message sent by the user in the bot's DMs
  pub client_id: ID,
  // Whether or not the message was marked for deletion
  pub is_deleted: bool,
  // The message mirrored by Modmail for the user (client_id)
  pub modmail_id: ID,
  // The Discord user that sent the message
  pub sender: ID,
  // The thread this message is in
  pub thread_id: ID,
  // Whether or not it's a message sent by user in the thread channel (staff)
  pub internal: bool,
}

pub struct User {
  pub id: ID,
}

pub struct Thread {
  // Creator of the thread
  pub author: User,
  // Category channel ID
  pub channel_id: ID,
  // Unique ID that represents this thread
  pub id: ID,
  // Whether this thread is still open
  pub is_active: bool,
  // Messages associated with this thread
  pub messages: Vec<Message>,
  // The category (guild) this thread is part of
  pub category_id: ID,
}

pub struct DBThread {
  pub author: String,
  pub channel_id: ID,
  pub id: ID,
  pub is_active: bool,
  pub category_id: ID,
}

pub struct MuteStatus {
  // Discord user that is muted
  pub user_id: ID,
  // Unix epoch timestamp in milliseconds
  pub till: u64,
  // Category muted from
  pub category_id: ID,
  // Human readable reason given by a staff member
  // if no reason was provided it'll either be empty
  // or populated with something like "No Reason"
  pub reason: String,
}

pub struct StandardReply {
  // The reply used
  pub reply: String,
  // A name that represents this standard reply
  pub name: String,
  // An ID that represents this standard reply
  pub id: ID,
}

pub enum RoleLevel {
  Admin,
  Mod,
}

impl RoleLevel {
  pub fn to_value(&self) -> &str {
    match *self {
      RoleLevel::Admin => "admin",
      RoleLevel::Mod => "mod",
    }
  }

  pub fn from_value(role: &mut str) -> RoleLevel {
    let lowered = role.to_lowercase();

    match lowered.as_str() {
      "admin" | "administrator" => RoleLevel::Admin,
      _ => RoleLevel::Mod,
    }
  }
}

pub struct Role {
  category_id: ID,
  role_id: ID,
  level: RoleLevel,
}
