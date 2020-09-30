export const Code = "stationletter"

export const Create = "create"

export const Audit = "audit"

export const View = "view"

//TODO: i18n
export const label = (codes = []) => {
  let [code = ""] = codes
  code = code.toLowerCase()

  switch (code) {
    case Create:
      return "新增站内信"
    case Audit:
      return "审核站内信"
    case View:
      return "查看站内信"
    default:
      return ""
  }
}
