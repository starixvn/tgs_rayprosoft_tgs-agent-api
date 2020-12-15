import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select as _Select } from "bumbag"
import { Field as _Field } from "../../select/Field"
import { Trans, useTrans } from "../../i18n"

const Disabled = 0
const DisabledName = "Disabled"
const DisabledTrans = {
  zh: "封锁",
  en: DisabledName
}

const Enabled = 1
const EnabledName = "Enabled"
const EnabledTrans = {
  zh: "活跃",
  en: "Enabled"
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

export const Select = ({ name, required, ...props }) => {
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
    <_Field name={name} required={required}>
      {(props) => <_Select options={options} {...props} />}
    </_Field>
  )
}
