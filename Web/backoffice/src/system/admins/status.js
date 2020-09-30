import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Select as _Select } from "bumbag"
import { Field as _Field } from "../../select/Field"

const Disabled = 0

const DisabledString = "Disabled"

const Enabled = 1

const EnabledString = "Enabled"

const Archived = 2

const Locked = 3

export const from = (value) => {
  if (value === DisabledString || value === `${Disabled}`) {
    return Disabled
  }
  if (value === EnabledString || value === `${Enabled}`) {
    return Enabled
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
    label: label(Disabled),
    value: Disabled
  },
  {
    label: label(Enabled),
    value: Enabled
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

  return value
}
