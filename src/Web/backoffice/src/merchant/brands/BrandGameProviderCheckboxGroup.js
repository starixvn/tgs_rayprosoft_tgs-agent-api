import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
  Table,
  Set,
  Heading,
  Text,
  useModalState,
  Icon,
  FieldStack,
  Box,
  Group,
  Button
} from "bumbag"
import { useFormContext, useFieldArray, Controller } from "react-hook-form"
import { isAllPropChecked } from "../../checkbox/isAllChecked"
import { Trans, useTrans } from "../../i18n"
import Checkbox, { CheckboxField } from "../../checkbox"
import Disclosure, { ConfigIconDisclosure } from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import Input, { InputField } from "../../input/Input"
import Select, { SelectField } from "../../select/Select"
import { Switch, SwitchField } from "../../switch/Switch"
import Backdrop from "../../dialog/Backdrop"
import {
  faDesktop,
  faMobile,
  faLanguage
} from "@fortawesome/free-solid-svg-icons"
import { ConfirmText } from "../../button/ActionButtons"
import { GameProviderConfigSelect } from "../../game/providers/configs/GameProviderConfigSelect"
import { GameProviderLanguageSelect } from "../../game/providers/GameProviderLanguageSelect"

export const BrandGameProviderCheckboxGroupTable = ({
  name = "gameProviders",
  fieldArrayContext,
  width = "100%",
  disabled = false,
  ...props
}) => {
  let { control, setValue, watch, register } = useFormContext()

  const { fields } = fieldArrayContext

  let selection = watch(name)
  let allChecked = isAllPropChecked(selection)

  return (
    <Set orientation="vertical" width={width}>
      <Table width={width}>
        <Table.Head>
          <Table.Row>
            <Table.HeadCell textAlign="center">
              <Checkbox
                checked={allChecked}
                onChange={(event) => {
                  let checked = event.target.checked
                  fields.forEach((field, index) => {
                    setValue(checkedFieldName(name, field, index), checked)
                  })
                }}
                disabled={disabled}
              />
            </Table.HeadCell>
            <Table.HeadCell textAlign="center">
              <Trans zh="游戏厂商" en="Game Provider" />
            </Table.HeadCell>
            <Table.HeadCell textAlign="center">
              <Trans zh="设定" en="Configuration" />
            </Table.HeadCell>
          </Table.Row>
        </Table.Head>
        <Table.Body>
          {fields.map((field, index) => (
            <Row
              key={field._id}
              name={name}
              disabled={disabled}
              field={field}
              index={index}
            />
          ))}
        </Table.Body>
      </Table>
    </Set>
  )
}

const Row = ({ name, disabled, field, index }) => {
  let formContext = useFormContext()
  let { register, setValue, control } = formContext

  let configDialog = useModalState()
  let defaultLangDialog = useModalState()
  let desktopQueryStringDialog = useModalState()
  let mobileQueryStringDialog = useModalState()
  return (
    <Table.Row>
      <Table.Cell textAlign="center">
        <Checkbox
          key={field._id}
          name={checkedFieldName(name, field, index)}
          checkboxRef={formContext?.register?.()}
          defaultChecked={field.checked}
          disabled={disabled}
        />
      </Table.Cell>
      <Table.Cell textAlign="center">{field.name}</Table.Cell>
      <Table.Cell textAlign="center">
        <Set orientation="horizontal" alignX="center">
          <Controller
            control={control}
            name={configFieldName(name, field, index)}
            defaultValue={field.configurationId}
            render={(
              { onChange, onBlur, value, name: configName, ref },
              { invalid, isTouched, isDirty }
            ) => {
              return (
                <>
                  <ConfigIconDisclosure {...configDialog} />
                  {configDialog.visible && (
                    <ConfigDialog
                      gameProvider={field}
                      configuration={value}
                      dialog={configDialog}
                      onChange={(configId) => {
                        setValue(configName, configId)
                      }}
                    />
                  )}
                </>
              )
            }}
          />
          <Controller
            control={control}
            name={langFieldName(name, field, index)}
            defaultValue={field.defaultLanguageCode ?? ""}
            render={(
              { onChange, onBlur, value, name: fieldName, ref },
              { invalid, isTouched, isDirty }
            ) => {
              return (
                <>
                  <Disclosure {...defaultLangDialog}>
                    <Icon icon={faLanguage} type="font-awesome" />
                  </Disclosure>
                  {defaultLangDialog.visible && (
                    <DefaultLangDialog
                      value={value}
                      gameProvider={field}
                      dialog={defaultLangDialog}
                      onChange={(code) => {
                        setValue(fieldName, code)
                      }}
                    />
                  )}
                </>
              )
            }}
          />
          <Controller
            control={control}
            name={queryStringFieldName(
              name,
              field,
              index,
              QueryStringType.Desktop
            )}
            defaultValue={field.desktopQueryString ?? ""}
            render={(
              { onChange, onBlur, value, name: fieldName, ref },
              { invalid, isTouched, isDirty }
            ) => {
              return (
                <>
                  <Disclosure {...desktopQueryStringDialog}>
                    <Icon icon={faDesktop} type="font-awesome" />
                  </Disclosure>
                  {desktopQueryStringDialog.visible && (
                    <QueryStringDialog
                      type={QueryStringType.Desktop}
                      value={value}
                      dialog={desktopQueryStringDialog}
                      onChange={(queryString) => {
                        setValue(fieldName, queryString)
                      }}
                    />
                  )}
                </>
              )
            }}
          />
          <Controller
            control={control}
            name={queryStringFieldName(
              name,
              field,
              index,
              QueryStringType.Mobile
            )}
            defaultValue={field.mobileQueryString ?? ""}
            render={(
              { onChange, onBlur, value, name: fieldName, ref },
              { invalid, isTouched, isDirty }
            ) => {
              return (
                <>
                  <Disclosure {...mobileQueryStringDialog}>
                    <Icon icon={faMobile} type="font-awesome" />
                  </Disclosure>
                  {mobileQueryStringDialog.visible && (
                    <QueryStringDialog
                      type={QueryStringType.Mobile}
                      value={value}
                      dialog={mobileQueryStringDialog}
                      onChange={(queryString) => {
                        setValue(fieldName, queryString)
                      }}
                    />
                  )}
                </>
              )
            }}
          />
        </Set>
      </Table.Cell>
    </Table.Row>
  )
}

const QueryStringType = {
  Desktop: "desktopQueryString",
  Mobile: "mobileQueryString"
}

const useQueryStringTitle = (type = QueryStringType.Desktop) => {
  let desktopTitle = useTrans({
    zh: `桌面搜索参数`,
    en: `Desktop Query String`
  })

  let mobileTitle = useTrans({
    zh: `移动搜索参数`,
    en: `Mobile Query String`
  })

  switch (type) {
    case QueryStringType.Desktop:
      return desktopTitle
    case QueryStringType.Mobile:
      return mobileTitle
  }
}

const QueryStringDialog = ({ type, value, dialog, onChange }) => {
  let title = useQueryStringTitle(type)
  return (
    <Backdrop {...dialog}>
      <FormDialog
        stopPropagation={true}
        onSubmit={(data) => {
          onChange(data.url)
          dialog.hide()
        }}
        submitText={<ConfirmText />}
        dialog={dialog}
        title={title}
        hideBackdrop={true}
        minWidth="500px"
      >
        <InputField label="Query String" name="url" defaultValue={value} />
      </FormDialog>
    </Backdrop>
  )
}

const ConfigDialog = ({ gameProvider, dialog, onChange }) => {
  let title = useTrans({
    zh: `游戏商配置`,
    en: `Game Provider Configuration`
  })

  return (
    <Backdrop {...dialog}>
      <FormDialog
        stopPropagation={true}
        onSubmit={({ id, ...data }) => {
          onChange(id)
          dialog.hide()
        }}
        submitText={<ConfirmText />}
        dialog={dialog}
        title={title}
        hideBackdrop={true}
        minWidth="max-content"
      >
        <GameProviderConfigSelect name="id" gameProviderId={gameProvider?.id} />
      </FormDialog>
    </Backdrop>
  )
}

const DefaultLangDialog = ({ gameProvider, dialog, onChange }) => {
  let title = useTrans({
    zh: `游戏商预设语言`,
    en: `Game Provider Default Language`
  })

  return (
    <Backdrop {...dialog}>
      <FormDialog
        stopPropagation={true}
        onSubmit={({ code }) => {
          onChange(code)
          dialog.hide()
        }}
        submitText={<ConfirmText />}
        dialog={dialog}
        title={title}
        hideBackdrop={true}
        minWidth="max-content"
      >
        <GameProviderLanguageSelect
          name="code"
          gameProviderId={gameProvider?.gameProviderId}
        />
      </FormDialog>
    </Backdrop>
  )
}

const checkedFieldName = (name, field, index) => `${name}[${index}].checked`

const queryStringFieldName = (name, field, index, type) =>
  `${name}[${index}].${type}`

const configFieldName = (name, field, index) =>
  `${name}[${index}].configurationId`

const langFieldName = (name, field, index) =>
  `${name}[${index}].defaultLanguageCode`
