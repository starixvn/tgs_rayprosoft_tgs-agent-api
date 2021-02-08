import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useGameProvider } from "./client"
import Select from "../../select/Select"
import { useTrans } from "../../i18n"

export const GameProviderLanguageSelect = ({
  name,
  gameProviderId,
  defaultValue,
  disabled,
  ...props
}) => {
  let { data, isLoading, error } = useGameProvider(gameProviderId)
  let options =
    data?.languages?.map(({ cultureCode, ...lang }) => ({
      label: cultureCode,
      value: cultureCode
    })) ?? []

  let selectConfigLabel = useTrans({
    zh: "选择游戏商语言",
    en: "Select Game Provider Language"
  })
  let placeholder = disabled
    ? options.find(({ value }) => defaultValue)?.label
    : selectConfigLabel
  return (
    <Select
      name={name}
      options={options}
      placeholder={placeholder ?? selectConfigLabel}
      isLoading={isLoading}
      disabled={disabled}
      {...props}
    />
  )
}
