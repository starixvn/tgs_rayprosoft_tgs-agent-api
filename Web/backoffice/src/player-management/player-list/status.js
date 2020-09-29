import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Select as _Select } from "bumbag"
import { Field as _Field } from "../../select/Field"

const Enabled = 0

const EnabledString = "Enabled"

const Disabled = 1

const DisabledString = "Disabled"

const InBlacklist = 2

const InBlacklistString = "InBlacklist"

const from = (value) => {
  if (value === EnabledString || value === `${Enabled}`) {
    return Enabled
  }
  if (value === DisabledString || value === `${Disabled}`) {
    return Disabled
  }
  if (value === InBlacklistString || value === `${InBlacklist}`) {
    return InBlacklist
  }
  return value
}

export const Select = ({ name, required, ...props }) => {
  return (
    <_Field name={name} required={required}>
      {(props) => <_Select options={statusOptions()} {...props} />}
    </_Field>
  )
}

const statusOptions = () => [
  {
    label: "",
    value: null
  },
  {
    label: label(Enabled),
    value: Enabled
  },
  {
    label: label(Disabled),
    value: Disabled
  },
  {
    label: label(InBlacklist),
    value: InBlacklist
  }
]

//TODO: i18n
export const label = (value) => {
  if (value === EnabledString || value === Enabled) {
    return "活跃"
  }

  if (value === DisabledString || value === Disabled) {
    return "封锁"
  }

  if (value === InBlacklistString || value === Enabled) {
    return "黑名单"
  }

  return value
}
