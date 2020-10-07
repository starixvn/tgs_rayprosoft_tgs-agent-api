import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Select as _Select } from "bumbag"
import { Field as _Field } from "../select/Field"

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

export const Field = ({ required, validate, ...props }) => {
  let { name } = props
  return (
    <_Field name={name} required={required}>
      {(props) => <Select {...props} />}
    </_Field>
  )
}

export const Select = ({ visible = true, ...props }) => {
  let options = [
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

  return <_Select options={[{ label: "", value: "" }, ...options]} {...props} />
}
