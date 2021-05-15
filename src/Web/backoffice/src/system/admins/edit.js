import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Stack, Box, Button, Text, Modal, Set } from "bumbag"
import { DepartmentSelect } from "../departments/DepartmentSelect"
import { RoleSelect } from "../roles/select"
import { Trans, useTrans } from "../../i18n"
import { AdminStatusSelect } from "./AdminStatus"
import { Create as CreateMenu, Update as UpdateMenu } from "./menu"
import {
    useIsDisplayMenuByRoute,
    useUserBrandId,
    useUserLicenseeId,
    useUserResellerId,
    useUserRoleType
} from "../../user/context"
import { useAddAdmin, useEditAdmin, useEditPassword } from "./client"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import Disclosure from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { PasswordInputField } from "../../input/PasswordInput"
import { useFormContext } from "react-hook-form"
import { InputField } from "../../input/Input"
import {
    adminOrNextRoleType,
    nextRoleType,
    RoleTypes,
    RoleTypeSelect,
    useRoleTypeOptions
} from "../roles/type"
import { MerchantIdSelect } from "../../merchant/MerchantSelect"
import { useRole } from "../roles/client"
import clone from "clone-deep"

export const AddButton = ({ onSubmitted, ...props }) => {
    let dialog = Modal.useState()

    let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="新增" en="Add" />
            </Disclosure>
            {dialog.visible && (
                <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
            )}
        </>
    )
}

export const EditButton = ({ user, onSubmitted, ...props }) => {
    let dialog = Modal.useState()

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
                    user={user}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

export const EditPasswordButton = ({ user, onSubmitted, ...props }) => {
    let dialog = Modal.useState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="修改密码" en="Change Password" />
            </Disclosure>
            {dialog.visible && (
                <AdminUserChangePasswordDialog
                    userId={user.id}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

export const AdminUserChangePasswordDialog = ({
    userId,
    onSubmitted,
    dialog,
    ...props
}) => {
    let [submitData, setSubmit] = useState()
    let { result, error, mutate, isLoading } = useEditPassword(
        userId,
        submitData
    )

    const clearResult = () => {
        setSubmit(null)
    }

    let title = usePasswordTitle()
    return (
        <FormDialog
            defaultValues={{
                password: "",
                confirmPassword: ""
            }}
            onSubmit={setSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={clearResult}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
        >
            <PasswordBody dialog={dialog} />
        </FormDialog>
    )
}

const PasswordBody = ({ dialog }) => {
    let { watch } = useFormContext()
    let password = watch("password")
    return (
        <Stack spacing="major-3" margin="major-1">
            <FieldValues>
                <Label name="password" required>
                    <Trans zh="密码" en="Password" />
                </Label>
                <Box>
                    <PasswordInputField name="password" autoComplete="off" />
                </Box>

                <Label name="confirmassword" required>
                    <Trans zh="确认密码" en="Confirm Password" />
                </Label>
                <Box>
                    <PasswordInputField
                        name="confirmPassword"
                        matchPassword={password}
                        autoComplete="off"
                    />
                </Box>
            </FieldValues>
        </Stack>
    )
}

const AddDialog = ({ onSubmitted, dialog, finalFocusRef, ...props }) => {
    let [submitData, setSubmit] = useState()
    let { result, error, mutate, isLoading } = useAddAdmin(submitData)

    const clearResult = () => {
        setSubmit(null)
    }

    let title = useCreateTitle()
    return (
        <FormDialog
            defaultValues={{
                username: "",
                departmentId: "",
                roleId: "",
                name: "",
                password: "",
                confirmPassword: ""
            }}
            onSubmit={setSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={clearResult}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            finalFocusRef={finalFocusRef}
            maxWidth="80vw"
            minWidth="55vw"
        >
            <AddBody dialog={dialog} />
        </FormDialog>
    )
}

const AddBody = ({}) => {
    let { watch, setValue } = useFormContext()
    let password = watch("password")

    let userResellerId = useUserResellerId()
    let [resellerId, setResellerState] = useState(userResellerId)
    const setResellerId = (value) => {
        if (!userResellerId) {
            //admin user
            setResellerState(value)
        }

        //ignore change
    }

    let userLicenseeId = useUserLicenseeId()
    let [licenseeId, setLicenseeState] = useState(userLicenseeId)
    const setLicenseeId = (value) => {
        if (!userLicenseeId) {
            //reseller user
            setLicenseeState(value)
        }

        //ignore change
    }

    let userBrandId = useUserBrandId()
    let [brandId, setBrandState] = useState(userBrandId)
    const setBrandId = (value) => {
        if (!userBrandId) {
            //licensee user
            setBrandState(value)
        }

        //ignore change
    }

    return (
        <Stack spacing="major-3" margin="major-1">
            <FieldValues>
                <UsernameLabel required />
                <Box>
                    <InputField name="username" required />
                </Box>

                <RoleLabel required />
                <RoleSelectGroup
                    resellerId={resellerId}
                    setResellerId={setResellerId}
                    licenseeId={licenseeId}
                    setLicenseeId={setLicenseeId}
                    brandId={brandId}
                    setBrandId={setBrandId}
                    emptyDepartment={() => {
                        setValue("departmentId", "")
                    }}
                />

                <DepartmentLabel required />
                <Box>
                    <DepartmentSelect
                        name="departmentId"
                        required
                        resellerId={resellerId}
                        licenseeId={licenseeId}
                        brandId={brandId}
                    />
                </Box>

                <NameLabel required />
                <Box>
                    <InputField name="name" required />
                </Box>

                <Label name="password" required>
                    <Trans zh="密码" en="Password" />
                </Label>
                <Box>
                    <PasswordInputField name="password" autoComplete="off" />
                </Box>

                <Label name="confirmPassword" required>
                    <Trans zh="确认密码" en="Confirm Password" />
                </Label>
                <Box>
                    <PasswordInputField
                        name="confirmPassword"
                        matchPassword={password}
                        autoComplete="off"
                    />
                </Box>
            </FieldValues>
        </Stack>
    )
}

const RoleSelectGroup = ({
    resellerId,
    setResellerId,
    licenseeId,
    setLicenseeId,
    brandId,
    setBrandId,
    emptyDepartment
}) => {
    let userRoleType = useUserRoleType()
    let { watch, setValue, getValues, reset } = useFormContext()
    let roleTypeOptions = useRoleTypeOptions()

    let roleType = watch("roleType")
    let roleId = watch("roleId")

    const onMerchantResetted = () => {
        setValue("roleId", "")
        emptyDepartment?.()
    }
    return (
        <Set orientation="vertical">
            {roleTypeOptions.length > 1 && (
                <Set orientation="horizontal">
                    <RoleTypeSelect
                        name="roleType"
                        defaultValue={roleType}
                        required
                        onChange={(event) => {
                            let role = event.target.value
                            setValue("roleType", role)

                            if (
                                !role ||
                                role === RoleTypes.Admin ||
                                role === RoleTypes.Reseller
                            ) {
                                setResellerId("")
                                setLicenseeId("")
                                setBrandId("")
                            }
                            if (role === RoleTypes.Licensee) {
                                setLicenseeId("")
                                setBrandId("")
                            }
                            if (role === RoleTypes.Brand) {
                                setBrandId("")
                            }
                            onMerchantResetted()
                            let values = getValues()
                            reset(values)
                        }}
                    />
                    {roleType && roleType != RoleTypes.Admin && (
                        <MerchantIdSelect
                            start={adminOrNextRoleType(userRoleType)}
                            end={roleType}
                            resellerProps={{
                                value: resellerId,
                                onChange: (value) => {
                                    setResellerId(value)
                                    onMerchantResetted()
                                }
                            }}
                            licenseeProps={{
                                value: licenseeId,
                                onChange: (value) => {
                                    setLicenseeId(value)
                                    onMerchantResetted()
                                }
                            }}
                            brandProps={{
                                value: brandId,
                                onChange: (value) => {
                                    setBrandId(value)
                                    onMerchantResetted()
                                }
                            }}
                        />
                    )}
                </Set>
            )}
            <RoleSelect
                name="roleId"
                defaultValue={roleId}
                required
                resellerId={resellerId}
                licenseeId={licenseeId}
                brandId={brandId}
            />
        </Set>
    )
}

const UsernameLabel = ({ name = "username", required }) => (
    <Label name={name} required={required}>
        <Trans zh="用户名" en="Username" />
    </Label>
)

const DepartmentLabel = ({ name = "departmentId", required }) => (
    <Label name={name} required={required}>
        <Trans zh="部门" en="Department" />
    </Label>
)

const RoleLabel = ({ name = "roleId", required }) => (
    <Label name={name} required={required}>
        <Trans zh="权限组" en="Role" />
    </Label>
)

const NameLabel = ({ name = "name", required }) => (
    <Label name={name} required={required}>
        <Trans zh="姓名" en="Name" />
    </Label>
)

const EditDialog = ({ user = {}, onSubmitted, dialog, ...props }) => {
    let [submitData, setSubmit] = useState()
    let { result, error, mutate, isLoading, deleteCache } = useEditAdmin(
        user?.id,
        submitData
    )

    const submit = (data) => {
        setSubmit(data)
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    let title = useUpdateTitle()

    let userRoleType = useUserRoleType()
    let roleType = userRoleType
    if (user?.resellerId) {
        roleType = RoleTypes.Reseller
    }
    if (user?.licenseeId) {
        roleType = RoleTypes.Licensee
    }
    if (user?.brandId) {
        roleType = RoleTypes.Brand
    }

    return (
        <FormDialog
            defaultValues={{ ...clone(user), roleType }}
            onSubmit={submit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            maxWidth="80vw"
            minWidth="55vw"
        >
            <EditBody user={user} dialog={dialog} />
        </FormDialog>
    )
}

const EditBody = ({ user }) => {
    let { watch, setValue, getValues } = useFormContext()

    let username = watch("username")
    let userResellerId = useUserResellerId()
    let [resellerId, setResellerState] = useState(user?.resellerId)
    const setResellerId = (value) => {
        if (!userResellerId) {
            //admin user
            setResellerState(value)
        }

        //ignore change
    }

    let userLicenseeId = useUserLicenseeId()
    let [licenseeId, setLicenseeState] = useState(user?.licenseeId)
    const setLicenseeId = (value) => {
        if (!userLicenseeId) {
            //reseller user
            setLicenseeState(value)
        }

        //ignore change
    }

    let userBrandId = useUserBrandId()
    let [brandId, setBrandState] = useState(user?.brandId)
    const setBrandId = (value) => {
        if (!userBrandId) {
            //licensee user
            setBrandState(value)
        }

        //ignore change
    }

    let departmentId = watch("departmentId")

    return (
        <Stack spacing="major-3" margin="major-1">
            <FieldValues>
                <UsernameLabel />
                <Box>
                    <InputField type="hidden" name="username" />
                    <InputField value={username} disabled />
                </Box>

                <RoleLabel />
                <RoleSelectGroup
                    resellerId={resellerId}
                    setResellerId={setResellerId}
                    licenseeId={licenseeId}
                    setLicenseeId={setLicenseeId}
                    brandId={brandId}
                    setBrandId={setBrandId}
                    emptyDepartment={() => {
                        setValue("departmentId", "")
                    }}
                />

                <DepartmentLabel />
                <Box>
                    <DepartmentSelect
                        name="departmentId"
                        defaultValue={departmentId}
                        required
                        resellerId={resellerId}
                        licenseeId={licenseeId}
                        brandId={brandId}
                    />
                </Box>

                <NameLabel />
                <Box>
                    <InputField name="name" required />
                </Box>

                <Label htmlFor="status">
                    <Trans zh="状态" en="Status" />
                    <Text color="danger">*</Text>
                </Label>
                <Box>
                    <AdminStatusSelect name="status" required />
                </Box>
            </FieldValues>
        </Stack>
    )
}

const useCreateTitle = () =>
    useTrans({
        zh: "新增管理员",
        en: "Add Admin"
    })

const useUpdateTitle = () =>
    useTrans({
        zh: "修改",
        en: "Edit"
    })

const usePasswordTitle = () =>
    useTrans({
        zh: "修改密码",
        en: "Change Password"
    })
