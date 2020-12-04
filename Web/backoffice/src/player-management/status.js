import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Select as _Select, Text } from "bumbag"
import { Field as _Field } from "../select/Field"
import { useTrans, Trans } from "../i18n"

const Enabled = "Enabled"
const EnabledTrans = {
  zh: "活跃",
  en: "Enabled"
}

const Disabled = "Disabled"
const DisabledTrans = {
  zh: "封锁",
  en: "Disabled"
}

const InBlacklist = "InBlacklist"
const InBlacklistTrans = {
  zh: "黑名单",
  en: "In Blacklist"
}

//TODO: rm?
/*
export const from = (value) => {
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

export const toString = (value) => {
  switch (value) {
    case Enabled:
      return EnabledString
    case Disabled:
      return DisabledString
    case InBlacklist:
      return InBlacklistString
    default:
      return value
  }
}
*/

export const Field = ({ name, required, ...props }) => {
  let options = [
    {
      label: useTrans(EnabledTrans),
      value: Enabled
    },
    {
      label: useTrans(DisabledTrans),
      value: Disabled
    },
    {
      label: useTrans(InBlacklistTrans),
      value: InBlacklist
    }
  ]

  if (!required) {
    options = [{ label: "", value: "" }, ...options]
  }

  return (
    <_Field name={name} required={required}>
      {(props) => <_Select options={options} {...props} />}
    </_Field>
  )
}

export const Label = ({ value }) => {
  let code = value
  switch (code) {
    case Enabled:
      return (
        <Text>
          <Trans {...EnabledTrans} />
        </Text>
      )
    case Disabled:
      return (
        <Text>
          <Trans {...DisabledTrans} />
        </Text>
      )
    case InBlacklist:
      return (
        <Text>
          <Trans {...InBlacklistTrans} />
        </Text>
      )
  }
  return null
}
