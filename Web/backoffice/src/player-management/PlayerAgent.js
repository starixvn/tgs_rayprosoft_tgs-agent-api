import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Text } from "bumbag"
import { useTrans, Trans } from "../i18n"
import Select from "../select/Select"

const On = Boolean(true).toString()

const Off = Boolean(false).toString()

export const PlayerAgentSelect = ({ placeholder, ...props }) => {
  let defaultPlaceholder = useTrans({
    zh: "代理",
    en: "Agent"
  })
  placeholder = placeholder ?? defaultPlaceholder

  let options = [
    {
      label: useTrans({
        zh: "开启",
        en: "Turn on"
      }),
      value: On
    },
    {
      label: useTrans({
        zh: "关闭",
        en: "Turn off"
      }),
      value: Off
    }
  ]

  return <Select options={options} {...props} />
}
