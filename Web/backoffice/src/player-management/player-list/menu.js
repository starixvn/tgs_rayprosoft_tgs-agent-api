export const Code = "playerlist"

export const View = "view"

export const Update = "update"

//TODO: i18n
export const label = (codes = []) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case View:
      return "查看玩家"
    case Update:
      return "编辑玩家"
    default:
      return ""
  }
}
