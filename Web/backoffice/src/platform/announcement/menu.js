export const Code = "announcement"

export const Create = "create"

export const Update = "update"

export const Audit = "audit"

export const View = "view"

//TODO: i18n
export const label = (codes = []) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Create:
      return "新增公告"
    case Update:
      return "修改公告"
    case Audit:
      return "审核公告"
    case View:
      return "查看公告"
    default:
      return ""
  }
}
