import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../../select/Select"
import { Locales, Trans, useTrans } from "../../../i18n"
import { useGameProviderConfigurationList } from "./client"

export const GameProviderConfigSelect = ({
  name,
  gameProviderId,
  defaultValue,
  disabled,
  ...props
}) => {
  let { items = [], isLoading, error } = useGameProviderConfigurationList({
    gameProviderId,
    showAll: true
  })

  let options =
    items?.map(({ name, id, ...item }) => ({
      label: name,
      value: id
    })) ?? []

  let selectConfigLabel = useTrans({
    zh: "选择游戏商配置",
    en: "Select Game Provider Configuration"
  })

  let placeholder = disabled
    ? options.find(({ value }) => defaultValue)?.label
    : selectConfigLabel

  return (
    <Select
      name={name}
      options={options}
      defaultValue={defaultValue}
      placeholder={placeholder ?? selectConfigLabel}
      isLoading={isLoading}
      disabled={disabled}
      {...props}
    />
  )
}
