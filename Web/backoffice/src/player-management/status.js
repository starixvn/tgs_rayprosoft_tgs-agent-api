import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Select as _Select, Text } from "bumbag"
import { Field as _Field } from "../select/Field"
import { useTrans, Trans } from "../i18n"

const Enabled = 0

const EnabledString = "Enabled"

const EnabledLabel = "Enabled"

const Disabled = 1

const DisabledString = "Disabled"

const DisabledLabel = "Disabled"

const InBlacklist = 2

const InBlacklistString = "InBlacklist"

const InBlacklistLabel = "In Blacklist"

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

export const Field = ({ name, required, ...props }) => {
  let enabledPair = useEnabledPair()
  let disabledPair = useDisabledPair()
  let blockedPair = useBlockedPair()

  let options = required 
  ? [enabledPair,
    disabledPair,
    blockedPair] 
  : [{
    label: "",
    value: null
  }, 
  enabledPair,
  disabledPair,
  blockedPair
  ]

  return (
    <_Field name={name} required={required}>
      {(props) => <_Select options={options} {...props} />}
    </_Field>
  )
}

const useEnabledPair = () => {
  let label = useTrans({
    zh: "活跃",
    en: EnabledLabel
  })

  return {
    label,
    value: `${Enabled}`
  }
}

const useDisabledPair = () => {
  let label = useTrans({
    zh: "封锁",
    en: DisabledLabel
  })

  return {
    label,
    value: `${Disabled}`
  }
}

const useBlockedPair = () => {
  let label = useTrans({
    zh: "黑名单",
    en: InBlacklistLabel
  })

  return {
    label,
    value: `${InBlacklist}`
  }
}

export const Label = ({ value }) => {
  let code = from(value)
  switch (code) {
    case Enabled:
      return (
        <Text>
          <Trans zh="活跃" en={EnabledString} />
        </Text>
      )
    case Disabled:
      return (
        <Text>
          <Trans zh="封锁" en={DisabledString} />
        </Text>
      )
    case InBlacklist:
      return (
        <Text>
          <Trans zh="黑名单" en={InBlacklistLabel} />
        </Text>
      )
  }
  return null
}
