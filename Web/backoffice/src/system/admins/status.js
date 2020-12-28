import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select as _Select } from "bumbag"
import { Field as _Field } from "../../select/Field"
import { Trans, useTrans } from "../../i18n"

const Disabled = "Disabled"
const DisabledTrans = {
  zh: "封锁",
  en: Disabled
}

const Enabled = "Enabled"
const EnabledTrans = {
  zh: "活跃",
  en: "Enabled"
}

export const Label = ({ value }) => {
  switch (value) {
    case Disabled:
      return <Trans {...DisabledTrans} />

    case Enabled:
      return <Trans {...EnabledTrans} />

    default:
      return value
  }
}

export const Select = ({ name, required, ...props }) => {
  let enabled = {
    label: useTrans(EnabledTrans),
    value: Enabled
  }

  let disabled = {
    label: useTrans(DisabledTrans),
    value: Disabled
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
