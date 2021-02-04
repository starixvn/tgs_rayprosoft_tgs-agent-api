import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useModalState, Stack, Heading, Box, Text, FieldStack } from "bumbag"
import { useIsDisplayMenuByRoute, useUserLicenseeId } from "../../user/context"
import Disclosure, { AddDisclosure } from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Update as UpdateMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import {
  CloseButtonFooter,
  ConfirmButtonFooter
} from "../../form/ConfirmButtonFooter"
import Input from "../../input/Input"
import { CurrencyCheckboxGroupTable } from "../../currency/CurrencyCheckboxGroup"
import { allCheckedKeys } from "../../checkbox/allCheckedKeys"
import { MerchantIdSelect } from "../MerchantSelect"
import { RoleTypes } from "../../system/roles/type"
import { useBrand, useUpdateBrand } from "./client"
import { useFormContext, useFieldArray } from "react-hook-form"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import Textarea from "../../textarea"
import { BrandLanguageCheckboxGroupTable } from "./BrandLanguageCheckboxGroup"
import { BrandGameProviderCheckboxGroupTable } from "./BrandGameProviderCheckboxGroup"
import { BrandBetLimitCheckboxGroupTable } from "./BrandBetLimitCheckboxGroup"
import useLicenseeBrand from "./useLicenseeBrand"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return null
  return (
    <>
      <AddDisclosure {...dialog} />
      {dialog.visible && (
        <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const AddDialog = ({ onSubmitted, dialog }) => {
  let title = useTrans({
    zh: "新增品牌",
    en: "Add Brand"
  })
  return (
    <FormDialog title={title} dialog={dialog}>
      <EditBody />
    </FormDialog>
  )
}

export const ViewButton = ({ record, ...props }) => {
  let dialog = useModalState()

  /* TODO: view auth?
  let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
  if (!isDisplay) {
    return null
  }
  */
  return (
    <>
      <Disclosure {...dialog} {...props}>
        <Trans zh="查看" en="View" />
      </Disclosure>
      {dialog.visible && (
        <ViewDialog
          id={record?.id}
          licenseeId={record?.licenseeId}
          dialog={dialog}
        />
      )}
    </>
  )
}

const ViewDialog = ({ id, licenseeId, dialog, ...props }) => {
  let {
    brand,
    licensee,
    languages,
    currencies,
    gameProviders,
    games,
    betLimits,
    isLoading,
    error,
    mutateBrand,
    deleteBrandCache
  } = useLicenseeBrand({ id, licenseeId })

  let title = useTrans({
    zh: "查看品牌",
    en: "View Brand"
  })

  if (isLoading) {
    return <LoadingBackdrop {...dialog} onClickClose={dialog.hide} />
  }

  if (error) {
    return <AlertDialog {...error} {...dialog} onClickClose={dialog.hide} />
  }

  const reset = () => {
    mutateBrand(undefined, false)
    deleteBrandCache()
  }

  let record = {
    ...brand,
    licensee,
    languages,
    currencies,
    gameProviders,
    games,
    betLimits
  }
  return (
    <FormDialog
      defaultValues={record}
      onClickClose={reset}
      title={title}
      dialog={dialog}
      showActionButtons={false}
      onClickClose={reset}
      footer={<CloseButtonFooter dialog={dialog} onClickClose={reset} />}
      placement="top"
      minWidth="60vw"
      maxWidth="unset"
      css={css`
        & .bb-DialogContent {
          position: relative;
        }
      `}
    >
      <EditBody readonly record={record} />
    </FormDialog>
  )
}

export const EditButton = ({ record, onSubmitted, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="修改" en="Edit" />
      </Disclosure>
      {dialog.visible && (
        <EditDialog
          id={record?.id}
          licenseeId={record?.licenseeId}
          onSubmitted={onSubmitted}
          dialog={dialog}
        />
      )}
    </>
  )
}

const EditDialog = ({ id, licenseeId, onSubmitted, dialog, ...props }) => {
  let {
    brand,
    licensee,
    languages,
    currencies,
    gameProviders,
    games,
    betLimits,
    isLoading: isLoadingBrand,
    error: brandError,
    mutateBrand,
    deleteBrandCache
  } = useLicenseeBrand({ id, licenseeId })

  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useUpdateBrand(id, formData)

  let title = useTrans({
    zh: "编辑品牌",
    en: "Edit Brand"
  })

  if (isLoadingBrand) {
    return <LoadingBackdrop {...dialog} />
  }

  if (brandError) {
    return (
      <AlertDialog {...brandError} {...dialog} onClickClose={dialog.hide} />
    )
  }

  const onSubmit = (data) => {
    let formData = {
      ...brand,
      ...normalizeFormData(data)
    }
    console.log("formData", formData)
    // setSubmit(formData)
  }

  const reset = () => {
    mutateBrand(undefined, false)
    deleteBrandCache()

    mutate(undefined, false)
    setSubmit(undefined)
  }

  let record = {
    ...brand,
    licensee,
    languages,
    currencies,
    gameProviders,
    games,
    betLimits
  }

  return (
    <FormDialog
      defaultValues={record}
      onSubmit={onSubmit}
      result={result}
      isLoading={isLoading}
      error={error}
      onCloseErrorAlert={reset}
      onSubmitted={() => {
        reset()
        onSubmitted?.()
      }}
      onClickClose={reset}
      title={title}
      showActionButtons={false}
      footer={
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
      }
      dialog={dialog}
      placement="top"
      minWidth="60vw"
      maxWidth="unset"
      css={css`
        & .bb-DialogContent {
          position: relative;
        }
      `}
    >
      <EditBody record={record} />
    </FormDialog>
  )
}

const normalizeFormData = ({
  languages,
  currencies,
  brandGameProviders,
  betLimits,
  ...data
}) => {
  languages = languages.filter(({ checked }) => checked)

  let currencyCodes = currencies
    .filter(({ checked }) => checked)
    .map(({ code }) => code)

  let gameProviders = Object.entries(brandGameProviders).reduce(
    (list, [key, { checked, ...props }]) => {
      if (checked) {
        return [...list, props]
      }

      return list
    },
    []
  )

  betLimits = betLimits
    .filter((betLimit) => betLimit.checked)
    .map(({ id, ...props }) => ({ id: parseInt(id), ...props }))

  return {
    ...data,
    languages,
    currencyCodes,
    gameProviders,
    betLimits
  }
}

const EditBody = ({ record, readonly, isAdd = false }) => {
  let { watch, getValues, register, control } = useFormContext()

  let userLicenseeId = useUserLicenseeId()

  let licensee = watch("licensee")

  let currencies = watch("currencies")
  let gameProviders = watch("gameProviders")
  let betLimits = watch("betLimits")

  let editable = !readonly
  return (
    <Stack orientation="vertical">
      <Heading use="h5" marginBottom="major-1">
        <Trans zh="基本资料" en="Basic Information" />
      </Heading>

      <FieldValues>
        <NormalLabel>
          <Trans zh="所属公司" en="Licensee" />
        </NormalLabel>
        <Box alignY="center">
          {isAdd && <MerchantIdSelect end={RoleTypes.Licensee} />}
          {!isAdd && <Text.Block>{licensee?.name}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="品牌代码" en="Code" />
        </NormalLabel>
        <Box alignY="center">
          <Input name="code" type={isAdd ? "input" : "hidden"} />
          {!isAdd && <Text.Block>{getValues("code")}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="品牌名称" en="Name" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Input name="name" required />}
          {readonly && <Text.Block>{record?.name}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="品牌简介" en="Description" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Textarea name="description" />}
          {readonly && <Text.Block>{record.description}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="品牌邮箱" en="Email" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Input type="email" name="operationalEmail" required />}
          {readonly && <Text.Block>{record.operationalEmail}</Text.Block>}
        </Box>

        {!isAdd && (
          <>
            <NormalLabel>
              <Trans zh="API密钥 " en="API Key" />
            </NormalLabel>
            <Box alignY="center">
              <Input
                name="apiKey"
                disabled
                readonly
                value={getValues("apiKey")}
              />
            </Box>
          </>
        )}

        <LanguageFieldsBox readonly={readonly} />

        <CurrencyFieldsBox readonly={readonly} />

        {gameProviders && (
          <>
            <NormalLabel>
              <Trans zh="游戏厂商" en="Game Provider" />
            </NormalLabel>
            <BrandGameProviderCheckboxGroupTable
              marginTop="major-1"
              disabled={readonly}
            />
          </>
        )}

        {betLimits && (
          <>
            <NormalLabel>
              <Trans zh="品牌限红" en="Brand Bet Limit" />
            </NormalLabel>
            <FieldStack marginTop="major-1">
              <BrandBetLimitCheckboxGroupTable disabled={readonly} />
            </FieldStack>
          </>
        )}
      </FieldValues>
    </Stack>
  )
}

const LanguageFieldsBox = ({ readonly }) => {
  let { watch, control } = useFormContext()

  let fieldsName = "languages"
  let fieldArrayContext = useFieldArray({
    control,
    name: fieldsName,
    keyName: "_id"
  })
  let languages = watch(fieldsName)

  return (
    languages && (
      <>
        <NormalLabel>
          <Trans zh="语言" en="Language" />
        </NormalLabel>
        <BrandLanguageCheckboxGroupTable
          fieldArrayContext={fieldArrayContext}
          disabled={readonly}
          showBrandCultureCode
        />
      </>
    )
  )
}

const CurrencyFieldsBox = ({ readonly }) => {
  let { watch, control } = useFormContext()

  let fieldsName = "currencies"
  let fieldArrayContext = useFieldArray({
    control,
    name: fieldsName,
    keyName: "_id"
  })
  let currencies = watch(fieldsName)

  return (
    currencies && (
      <>
        <NormalLabel>
          <Trans zh="币种" en="Currency" />
        </NormalLabel>
        <CurrencyCheckboxGroupTable
          fieldArrayContext={fieldArrayContext}
          disabled={readonly}
        />
      </>
    )
  )
}
