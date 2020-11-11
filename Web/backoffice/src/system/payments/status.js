import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Select as _Select } from "bumbag"
import { Field as _Field } from "../../select/Field"
import { Trans, useTrans } from "../../i18n"

const Enabled = 0
const EnabledName = "Enabled"
const EnabledTrans = {
  zh: "开启",
  en: "Enabled"
}

const Disabled = 1
const DisabledName = "Disabled"
const DisabledTrans = {
  zh: "关闭",
  en: DisabledName
}

//string -> number
export const from = (value) => {
  switch (value) {
    case DisabledName:
    case `${Disabled}`:
      return Disabled

    case EnabledName:
    case `${Enabled}`:
      return Enabled

    default:
      return value
  }
}

//number -> string
export const toString = (value) => {
  switch (value) {
    case Enabled:
      return EnabledName
    case Disabled:
      return DisabledName
    default:
      return typeof value === "string" ? value : ""
  }
}

export const Label = ({ value }) => {
  switch (value) {
    case Disabled:
    case DisabledName:
      return <Trans {...DisabledTrans} />

    case Enabled:
    case EnabledName:
      return <Trans {...EnabledTrans} />

    default:
      return value
  }
}

export const Select = ({
  name,
  required,
  defaultValue = undefined,
  value = undefined,
  ...props
}) => {
  if (Number.isFinite(defaultValue)) {
    defaultValue = toString(defaultValue)
  }
  if (Number.isFinite(value)) {
    value = toString(value)
  }

  let enabled = {
    label: useTrans(EnabledTrans),
    value: EnabledName
  }

  let disabled = {
    label: useTrans(DisabledTrans),
    value: DisabledName
  }

  let options = required
    ? [enabled, disabled]
    : [
        {
          label: "",
          value: null
        },
        enabled,
        disabled
      ]
  return (
    <_Select
      options={options}
      defaultValue={defaultValue}
      value={value}
      {...props}
      onChange={(event) => props?.onChange(from(event.target.value))}
    />
  )
}
