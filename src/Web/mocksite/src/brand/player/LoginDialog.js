import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
    Table,
    Set,
    Button,
    Modal,
    Dialog,
    Card,
    Box,
    Input,
    Group,
    Icon,
    Stack,
    FieldStack,
    InputField,
    Text
} from "bumbag"
import { faPencilAlt } from "@fortawesome/free-solid-svg-icons"
import { useCaptcha, usePlayerToken } from "../../client/brand/player"
import { useRouteMatch, useHistory } from "react-router-dom"
import { useForm } from "react-hook-form"
import { Type as PlatformType, platformOf } from "../../platform"
import { PlatformSelect } from "../../platform/PlatformSelect"

export const LoginDialogButton = ({
    brandCode,
    currency,
    brandToken,
    username,
    ...props
}) => {
    let desktopDialog = Modal.useState()
    let mobileDialog = Modal.useState()
    let manualDialog = Modal.useState()
    return (
        <>
            <Group>
                <Modal.Disclosure use={Button} {...desktopDialog}>
                    Desktop Login
                </Modal.Disclosure>
                <Modal.Disclosure use={Button} {...mobileDialog}>
                    Mobile Login
                </Modal.Disclosure>
                <Modal.Disclosure use={Button} {...manualDialog}>
                    <Icon type="font-awesome" icon={faPencilAlt} />
                </Modal.Disclosure>
            </Group>
            {desktopDialog.visible && (
                <LoginDialog
                    brandCode={brandCode}
                    currency={currency}
                    brandToken={brandToken}
                    username={username}
                    platform={PlatformType.Desktop}
                    auto={true}
                    dialog={desktopDialog}
                />
            )}
            {mobileDialog.visible && (
                <LoginDialog
                    brandCode={brandCode}
                    currency={currency}
                    brandToken={brandToken}
                    username={username}
                    platform={PlatformType.Mobile}
                    auto={true}
                    dialog={mobileDialog}
                />
            )}
            {manualDialog.visible && (
                <LoginDialog
                    brandCode={brandCode}
                    currency={currency}
                    brandToken={brandToken}
                    username={username}
                    auto={false}
                    dialog={manualDialog}
                />
            )}
        </>
    )
}

const DefaultPassword = "TgsP@ssw0rd"
const LoginDialog = ({
    brandCode,
    currency,
    brandToken,
    username,
    platform = PlatformType.Desktop,
    auto,
    dialog,
    ...props
}) => {
    let match = useRouteMatch()

    let history = useHistory()

    let {
        register,
        handleSubmit,
        watch,
        setError,
        setValue,
        errors
    } = useForm()

    let [platformtype, setPlatform] = useState(platform)
    let [fromData, setSubmit] = useState(
        auto ? undefined : { bypassCaptcha: false }
    )

    let { token, error, isLoading, mutate } = usePlayerToken(
        dialog.visible
            ? { brandCode, currency, username, platformtype }
            : undefined,
        fromData
    )
    const onSubmit = handleSubmit((data) => {
        console.log("platformtype", platformtype)
        mutate(undefined, false)
        setSubmit({
            platformtype,
            ...data
        })
    })

    useEffect(() => {
        if (token) {
            let platyformString = platformOf(platformtype)
            let url = `${match.url}/${username}/${platyformString}`
            history.push(url)
            dialog.hide()
        }
    }, [token])

    const errorDialog = Modal.useState()
    const PasswordError = 103
    // const CaptchaAnsError = 31
    useEffect(() => {
        if (error && !token) {
            if (fromData?.username) {
                //not auto login
                errorDialog.show()
            }

            if (error.err === PasswordError) {
                if (auto && !fromData?.username) {
                    setValue("password", "", { shouldValidate: true })
                }
                setError("password", {
                    type: "password",
                    shouldFocus: true
                })
            }
        }
    }, [error])

    return (
        <>
            <Dialog.Modal
                showActionButtons
                title="Login"
                showCloseButton
                hideOnClickOutside={false}
                actionButtonsProps={{
                    isLoading,
                    onClickSubmit: onSubmit
                }}
                {...dialog}
            >
                <form onSubmit={onSubmit}>
                    <Stack>
                        <FieldStack>
                            <InputField
                                name="username"
                                inputRef={register}
                                label="Username"
                                defaultValue={username}
                                readOnly={true}
                            />
                            <InputField
                                name="password"
                                inputRef={register({ required: true })}
                                label="Password"
                                defaultValue={DefaultPassword}
                            />
                            <PlatformSelect
                                value={platformtype}
                                onChange={(event) => {
                                    let value = event.target.value
                                    setPlatform(parseInt(value))
                                }}
                            />
                            {/* <InputField
                name="answer"
                inputRef={register({ required: true })}
                state={errors.answer ? "danger" : ""}
                label="Captcha"
                addonBefore={
                  <Button
                    onClick={() => mutateCaptcha(undefined)}
                    isLoading={isLoadingCaptcha}
                  >
                    {image ? (
                      <>
                        <img src={`data:image/png;base64, ${image}`} />
                        <input
                          name="captcha"
                          ref={register}
                          value={captcha}
                          readOnly
                          css={css`
                            display: none;
                          `}
                        />
                      </>
                    ) : (
                      "..."
                    )}
                  </Button>
                }
              /> */}
                        </FieldStack>
                    </Stack>
                </form>
            </Dialog.Modal>
            {error && (
                <Modal.Backdrop zIndex="19900410" {...errorDialog}>
                    <Dialog.Modal
                        title="Error"
                        showCloseButton
                        type="warning"
                        hideBackdrop={true}
                        {...errorDialog}
                    >
                        <Text.Block>{JSON.stringify(error)}</Text.Block>
                    </Dialog.Modal>
                </Modal.Backdrop>
            )}
        </>
    )
}
