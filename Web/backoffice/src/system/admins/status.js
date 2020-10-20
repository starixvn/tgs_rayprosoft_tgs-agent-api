import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Select as _Select, Text } from "bumbag"
import { Field as _Field } from "../../select/Field"
import { useTrans } from "../../i18n"

const Disabled = 0

const DisabledString = "Disabled"

const Enabled = 1

const EnabledString = "Enabled"

const Archived = 2

const ArchivedString = "Archived"

const Locked = 3

const LockedString = "Locked"

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
  let enabledString = useEnabledLabel()
  let enabled = {
    label: enabledString,
    value: EnabledString
  }

  let disabledString = useDisabledLabel()
  let disabled = {
    label: disabledString,
    value: DisabledString
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

const useEnabledLabel = () =>
  useTrans({
    zh: "活跃",
    en: EnabledString
  })

const useDisabledLabel = () =>
  useTrans({
    zh: "封锁",
    en: DisabledString
  })

export const Label = ({ value }) => {
  let enabledLabel = useEnabledLabel()
  let disabledLabel = useDisabledLabel()

  if (value === EnabledString) {
    return <Text.Block>{enabledLabel}</Text.Block>
  }

  if (value === DisabledString) {
    return <Text.Block>{disabledLabel}</Text.Block>
  }

  if (value === ArchivedString) {
    //TODO: i18n
    return <Text.Block>{ArchivedString}</Text.Block>
  }

  if (value === LockedString) {
    //TODO: i18n
    return <Text.Block>{LockedString}</Text.Block>
  }

  return ""
}
