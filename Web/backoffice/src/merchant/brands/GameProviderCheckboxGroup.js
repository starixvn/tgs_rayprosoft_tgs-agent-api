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
import Disclosure from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import Input, { InputField } from "../../input/Input"
import Select, { SelectField } from "../../select/Select"
import { Switch, SwitchField } from "../../switch/Switch"
import Backdrop from "../../dialog/Backdrop"
import { faCog, faDesktop, faMobile } from "@fortawesome/free-solid-svg-icons"
import { ConfirmText } from "../../button/ActionButtons"

export const BrandGameProviderCheckboxGroupTable = ({
  name = "brandGameProviders",
  fieldArrayName = "gameProviders",
  width = "100%",
  disabled = false,
  ...props
}) => {
  let configDialog = useModalState()
  let desktopQueryStringDialog = useModalState()
  let mobileQueryStringDialog = useModalState()

  let { control, setValue, watch, register } = useFormContext()
  let gameProviders = watch(fieldArrayName)
  const { fields } = useFieldArray({
    control,
    name: fieldArrayName,
    keyName: "_id"
  })

  let selection = watch(name) ?? {}
  let allChecked = isAllPropChecked(selection)

  console.log("brandGameProviders", watch("brandGameProviders"))
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
                    setValue(checkedFieldName(name, field), checked)
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
          {fields.map((field) => (
            <Row
              key={field._id}
              name={name}
              disabled={disabled}
              field={field}
              configDialog={configDialog}
              desktopQueryStringDialog={desktopQueryStringDialog}
              mobileQueryStringDialog={mobileQueryStringDialog}
            />
          ))}
        </Table.Body>
      </Table>
    </Set>
  )
}

const Row = ({
  name,
  disabled,
  field,
  configDialog,
  desktopQueryStringDialog,
  mobileQueryStringDialog
}) => {
  let formContext = useFormContext()
  let { register, unregister, setValue, control } = formContext
  return (
    <Table.Row>
      <Table.Cell textAlign="center">
        <Checkbox
          key={field._id}
          name={checkedFieldName(name, field)}
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
            name={configFieldName(name, field)}
            render={(
              { onChange, onBlur, value, name: configName, ref },
              { invalid, isTouched, isDirty }
            ) => {
              return (
                <>
                  <Disclosure {...configDialog}>
                    <Icon icon={faCog} type="font-awesome" />
                  </Disclosure>
                  {configDialog.visible && (
                    <ConfigDialog
                      gameProvider={field}
                      configuration={value}
                      dialog={configDialog}
                      onChange={(data) => {
                        setValue(configName, data)
                      }}
                    />
                  )}
                </>
              )
            }}
          />
          <Controller
            control={control}
            name={queryStringFieldName(name, field, QueryStringType.Desktop)}
            render={(
              { onChange, onBlur, value, name: fieldName, ref },
              { invalid, isTouched, isDirty }
            ) => {
              return (
                <>
                  <Disclosure {...desktopQueryStringDialog}>
                    <Icon icon={faDesktop} type="font-awesome" />
                  </Disclosure>
                  <QueryStringDialog
                    type={QueryStringType.Desktop}
                    value={value}
                    dialog={desktopQueryStringDialog}
                    onChange={(queryString) => {
                      setValue(fieldName, queryString)
                    }}
                  />
                </>
              )
            }}
          />
          <Controller
            control={control}
            name={queryStringFieldName(name, field, QueryStringType.Mobile)}
            render={(
              { onChange, onBlur, value, name: fieldName, ref },
              { invalid, isTouched, isDirty }
            ) => {
              return (
                <>
                  <Disclosure {...mobileQueryStringDialog}>
                    <Icon icon={faMobile} type="font-awesome" />
                  </Disclosure>
                  <QueryStringDialog
                    type={QueryStringType.Mobile}
                    value={value}
                    dialog={mobileQueryStringDialog}
                    onChange={(queryString) => {
                      setValue(fieldName, queryString)
                    }}
                  />
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
    zh: `配置`,
    en: `Configuration`
  })
  return (
    <Backdrop {...dialog}>
      <FormDialog
        onSubmit={(data) => {
          onChange(data)
          dialog.hide()
        }}
        submitText={<ConfirmText />}
        dialog={dialog}
        title={title}
        hideBackdrop={true}
        minWidth="500px"
      >
        <FieldStack>
          <NameInputField />
          <DescriptionInputField />
          <FieldStack orientation="horizontal">
            <DefaultSwitchField />
            <ConfigurationStatusSelectField />
          </FieldStack>
          <InputField name="clientId" label="Client Id" />
          <InputField name="secretKey" label="Secret Key" />

          <InputField name="desktopEndpoint" label="Desktop Endpoint" />
          <InputField name="mobileEndpoint" label="Mobile Endpoint" />
          {/* TODO: json textarea editor */}
          <InputField name="configurationData" label="Configuration Data" />
        </FieldStack>
      </FormDialog>
    </Backdrop>
  )
}

const NameInputField = ({ name = "name", ...props }) => {
  let label = useTrans({
    zh: "名称",
    en: "Name"
  })

  return <InputField name={name} label={label} {...props} />
}

const DescriptionInputField = ({ name = "description", ...props }) => {
  let label = useTrans({
    zh: "说明",
    en: "Description"
  })

  return <InputField name={name} label={label} {...props} />
}

const DefaultSwitchField = ({ name = "isDefault" }) => {
  let label = useTrans({
    zh: "预设",
    en: "Default"
  })

  return <SwitchField name={name} label={label} />
}

const Active = "Active"
const ActiveTrans = {
  zh: "开启",
  en: "Enabled"
}

const Inactive = "Inactive"
const InactiveTrans = {
  zh: "关闭",
  en: Inactive
}

const Maintenance = "Maintenance"
const MaintenanceTrans = {
  zh: "维护",
  en: Maintenance
}
const ConfigurationStatusSelectField = ({ name = "status", ...props }) => {
  let options = [
    {
      label: useTrans(ActiveTrans),
      value: Active
    },
    {
      label: useTrans(InactiveTrans),
      value: Inactive
    },
    {
      label: useTrans(MaintenanceTrans),
      value: Maintenance
    }
  ]

  let label = useTrans({
    zh: "状态",
    en: "Status"
  })
  let placeholder = useTrans({
    zh: "选择状态",
    en: "Select Status"
  })
  return (
    <SelectField
      name={name}
      placeholder={placeholder}
      label={label}
      options={options}
      {...props}
    />
  )
}

const checkedFieldName = (name, checkboxField) =>
  `${name}.${checkboxField.id}.checked`

const queryStringFieldName = (name, field, type) =>
  `${name}.${field.id}.${type}`

const configFieldName = (name, field) => `${name}.${field.id}.configuration`
