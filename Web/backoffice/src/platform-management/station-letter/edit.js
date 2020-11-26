import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field, Form, useFormikContext } from "formik"
import {
  Stack,
  Box,
  Button,
  Text,
  Modal,
  Textarea,
  TextareaField,
  Input,
  Set,
  Popover,
  List
} from "bumbag"
import {
  Create as CreateMenu,
  Verify as VerifyMenu,
  View as ViewMenu
} from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { FieldValues } from "../../layout/Grid"
import FormDialog from "../../form/Dialog"
import Label from "../../form/Label"
import Disclosure from "../../dialog/Disclosure"
import { Trans, useTrans, useLocale, Locales } from "../../i18n"
import { Field as InputField } from "../../input/Field"
import {
  useAddStationLetter,
  useStationLetterUpload,
  useVerifyStationLetter
} from "./client"
import { CancelText } from "../../button/ActionButtons"
import TargetSelectField, {
  AllPlayers,
  DesignatedPlayers
} from "./TargetSelectField"
import Alert from "../../dialog/Alert"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = Modal.useState()
  let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog} {...props}>
        <Trans zh="新增" en="Add" />
      </Disclosure>
      {dialog.visible && (
        <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const AddDialog = ({ onSubmitted, dialog, finalFocusRef, ...props }) => {
  let [data, setSubmit] = useState()
  let { result, isLoading, error } = useAddStationLetter(data)

  const clearResult = () => {
    setSubmit(null)
  }

  let title = useTrans({
    zh: "新增站内信",
    en: "Add Station Letter"
  })

  return (
    <FormDialog
      initialValues={{
        target: AllPlayers,
        subject: "",
        content: ""
      }}
      onSubmit={({ target, ...data }) => {
        if (target === AllPlayers) {
          data.assignedAllPlayers = true
        }

        setSubmit(data)
      }}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
      finalFocusRef={finalFocusRef}
      placement="top"
      css={DialogCss}
    >
      <EditBody dialog={dialog} />
    </FormDialog>
  )
}

const DialogCss = css`
  min-width: 65vw;
  min-height: 80vh;

  & .bb-DialogContent {
    min-height: 80vh;
  }

  .tox-fullscreen & {
    top: 0;
    left: 0;
    transform: none;
  }
`

const EditBody = ({ readonly = false, dialog }) => {
  let {
    values,
    setFieldValue,
    isSubmitting,
    ...formContext
  } = useFormikContext()
  let contentEditorRef = useRef()
  let required = !readonly
  let locale = useLocale()
  let language = locale === Locales.zh ? "zh_CN" : ""
  let [editor, setEditor] = useState()
  let uploadInputRef = useRef()

  let [formData, setFormData] = useState()
  let { result, isLoading, error, deleteCache } = useStationLetterUpload(
    formData
  )

  const resetFormData = () => {
    if (uploadInputRef?.current) {
      uploadInputRef.current.value = ""
    }
    deleteCache()
    setFormData(undefined)
  }

  useEffect(() => {
    if (Array.isArray(result?.players) && !error) {
      setFieldValue(
        "playerUsernames",
        result?.players.map((item) => item.username)
      )
    }
  }, [formData, result])

  const setupEditor = () => {
    if (!contentEditorRef.current) {
      return
    }

    let selector = `textarea.${contentEditorRef.current.className}`
    tinymce.remove(selector)

    let options = {
      selector,
      readonly,
      base_url: "/tinymce",
      height: readonly ? 280 : 380,
      plugins:
        "preview paste importcss searchreplace autolink code visualchars fullscreen image link media codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists imagetools textpattern noneditable charmap emoticons",
      menubar: "file edit insert view format table upload",
      menu: {
        upload: { title: "Upload", items: "image", icon: "upload" }
      },
      toolbar: [
        "searchreplace | bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | outdent indent | undo redo | removeformat superscript subscript code codesample",
        "hr numlist bullist | link insertfile image preview anchor pagebreak insertdatetime media table | emoticons forecolor backcolor fullscreen "
      ],
      toolbar_mode: "sliding",
      images_upload_url: "/api/file/uploadFileS3",
      setup: (editor) => {
        editor.on("Change", (event) => {
          let content = event.target.getContent()
          setFieldValue("content", content)
        })
      }
    }
    if (language) {
      options.language = language
    }

    tinymce.init(options).then(([instance]) => {
      setEditor(instance)
    })
  }

  useEffect(() => {
    if (dialog.visible && contentEditorRef.current) {
      setupEditor()
    }
  }, [dialog.visible, contentEditorRef.current])

  return (
    <FieldValues>
      <Label name="target" required={required}>
        <Trans zh="对象" en="Target" />
      </Label>
      <Set>
        <TargetSelectField
          name="target"
          required={required}
          disabled={readonly}
        />
        {!readonly && values.target === DesignatedPlayers && (
          <>
            <input
              ref={uploadInputRef}
              type="file"
              onChange={(event) => {
                let [file] = event.target.files
                if (file) {
                  let formData = new FormData()
                  formData.append("file", file, file.name)
                  setFormData(formData)
                } else {
                  resetFormData()
                }
              }}
              css={css`
                display: none;
              `}
            />
            <Button
              type="button"
              palette="primary"
              size="small"
              onClick={(event) => {
                uploadInputRef?.current.click()
              }}
              isLoading={isLoading}
              disabled={isLoading}
            >
              <Trans zh="上传文件" en="Upload file" />
            </Button>
            <Button
              use="a"
              href="/download/payments/list.xlsx"
              download="players.xlsx"
              palette="primary"
              size="small"
            >
              <Trans zh="下载文件格式" en="Download Format File" />
            </Button>
            {error && <Alert {...error} onClickClose={resetFormData} />}
          </>
        )}
        {readonly &&
          values.target === DesignatedPlayers &&
          values.playerUsernames.length > 0 && (
            <Popover.State>
              <Popover.Disclosure
                use={Button}
                palette="primary"
                background="primary"
              >
                <Trans zh="玩家列表" en="Players" />
              </Popover.Disclosure>
              <Popover hasArrow>
                <List maxHeight="15em" overflow="auto">
                  {values.playerUsernames.map((name, index) => (
                    <List.Item key={index}>{name}</List.Item>
                  ))}
                </List>
              </Popover>
            </Popover.State>
          )}
      </Set>

      <Label name="subject" required={required}>
        <Trans zh="主题" en="Subject" />
      </Label>
      <InputField name="subject" required={required} disabled={readonly} />

      <Label name="content" required={required}>
        <Trans zh="内容" en="Content" />
      </Label>
      <Field
        name="content"
        validate={(value) => {
          if (required && !value) {
            return "Required"
          }
          return ""
        }}
      >
        {({ field, form, meta }) => {
          let state = meta.error && meta.touched && "danger"
          useEffect(() => {
            if (isSubmitting && meta.error) {
              editor?.focus()
            }
          }, [isSubmitting])
          return (
            <>
              <Global
                styles={css`
                  body .tox-tinymce-aux {
                    z-index: 19900420;
                  }
                `}
              />
              <textarea
                id="content"
                name="content"
                ref={contentEditorRef}
                required={required}
                disabled={readonly}
                css={css`
                  display: none;
                `}
                defaultValue={values?.content ?? ""}
              ></textarea>
            </>
          )
        }}
      </Field>
    </FieldValues>
  )
}

export const ViewButton = ({ record, ...props }) => {
  let dialog = Modal.useState()

  let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog} {...props}>
        <Trans zh="查看" en="View" />
      </Disclosure>
      {dialog.visible && <ViewDialog record={record} dialog={dialog} />}
    </>
  )
}

const ViewDialog = ({ record, dialog, finalFocusRef, ...props }) => {
  let title = useTrans({
    zh: "查看站内信",
    en: "View Station Letter"
  })

  return (
    <FormDialog
      initialValues={{
        ...record,
        target: record.assignedAllPlayers ? AllPlayers : DesignatedPlayers
      }}
      title={title}
      placement="top"
      css={DialogCss}
      dialog={dialog}
      showActionButtons={false}
      footer={
        <Box alignX="right" width="100%">
          <Button onClick={dialog.hide}>
            <CancelText />
          </Button>
        </Box>
      }
    >
      <VerifyBody readonly={true} dialog={dialog} />
    </FormDialog>
  )
}

export const VerifyButton = ({ record, onSubmitted, ...props }) => {
  let dialog = Modal.useState()

  let isDisplay = useIsDisplayMenuByRoute([VerifyMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog} {...props}>
        <Trans zh="审核" en="Verify" />
      </Disclosure>
      {dialog.visible && (
        <VerifyDialog
          record={record}
          onSubmitted={onSubmitted}
          dialog={dialog}
        />
      )}
    </>
  )
}

const VerifyDialog = ({
  record,
  onSubmitted,
  dialog,
  finalFocusRef,
  ...props
}) => {
  let [type, setType] = useState()
  let [data, setSubmit] = useState()
  let { result, error, mutate, isLoading } = useVerifyStationLetter({
    id: record?.id,
    type,
    data
  })

  const clearResult = () => {
    setSubmit(null)
  }

  let title = useTrans({
    zh: "审核站内信",
    en: "Verify Station Letter"
  })

  return (
    <FormDialog
      initialValues={{
        ...record,
        target: record.assignedAllPlayers ? AllPlayers : DesignatedPlayers,
        remarks: ""
      }}
      onSubmit={setSubmit}
      clickSubmit={({ submitForm }) => (event) => {
        setType("pass")
        submitForm()
      }}
      addonButtons={({ submitForm }) => {
        return (
          <Button
            onClick={() => {
              setType("reject")
              submitForm()
            }}
            palette="danger"
            variant="outlined"
          >
            <Trans zh="拒绝" en="Reject" />
          </Button>
        )
      }}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      placement="top"
      css={DialogCss}
      dialog={dialog}
    >
      <VerifyBody dialog={dialog} readonly={false} />
    </FormDialog>
  )
}

const VerifyBody = ({ readonly = false, dialog }) => {
  return (
    <Stack spacing="major-2">
      <EditBody readonly={true} dialog={dialog} />
      <Box border="default" height="1px" width="100%"></Box>
      <FieldValues>
        <Label name="remarks">
          <Trans zh="备注" en="Remarks" />
        </Label>
        <Field
          name="remarks"
          component={TextareaField.Formik}
          disabled={readonly}
        />
      </FieldValues>
    </Stack>
  )
}
