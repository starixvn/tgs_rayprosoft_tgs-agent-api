const Enabled = 0

const EnabledString = "Enabled"

const Disabled = 1

const DisabledString = "Disabled"

const InBlacklist = 2

const InBlacklistString = "InBlacklist"

//TODO: i18n
export const label = (value) => {
  if (value === EnabledString || value === Enabled) {
    return "活跃"
  }

  if (value === DisabledString || value === Disabled) {
    return "封锁"
  }

  if (value === InBlacklist || value === InBlacklistString) {
    return "黑名单"
  }

  return value
}
