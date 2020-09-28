export const Create = "create"

export const Update = "update"

export const Delete = "delete"

export const SendMessage = "sendmessage"

//TODO: i18n
export const label = (codes = []) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Create:
      return "新增用户"
    case Update:
      return "编辑用户"
    case Delete:
      return "删除用户"
    case SendMessage:
      return "发短信功能"
    default:
      return ""
  }
}
