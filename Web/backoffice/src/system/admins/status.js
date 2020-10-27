import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
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

const Archived = 2
const ArchivedName = "Archived"
const ArchivedTrans = {
  zh: "封存",
  en: "Archived"
}

const Locked = 3
const LockedName = "Locked"
const LockedTrans = {
  zh: "锁定",
  en: "Locked"
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

    case ArchivedName:
    case `${Archived}`:
      return Archived

    case LockedName:
    case `${Locked}`:
      return Locked

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

    case Archived:
    case ArchivedName:
      return <Trans {...ArchivedTrans} />

    case Locked:
    case LockedName:
      return <Trans {...LockedTrans} />

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

  let locked = {
    label: useTrans(LockedTrans),
    value: LockedName
  }

  let options = required
    ? [enabled, disabled, locked]
    : [
        {
          label: "",
          value: null
        },
        enabled,
        disabled,
        locked
      ]
  return (
    <_Field name={name} required={required}>
      {(props) => <_Select options={options} {...props} />}
    </_Field>
  )
}
