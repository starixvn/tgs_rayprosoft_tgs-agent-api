import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
    Table,
    Set,
    Button,
    Stack,
    FieldStack,
    Box,
    PageWithHeader,
    TopNav,
    DropdownMenu,
    Group,
    Card,
    Dialog,
    Modal,
    Text,
    FieldWrapper,
    InputField,
    Link as _Link
} from "bumbag"
import {
    Switch,
    Route,
    Link,
    useParams,
    useRouteMatch,
    useLocation,
    useHistory
} from "react-router-dom"
import {
    State,
    useCurrency,
    useLang,
    useSetCurrency,
    useSetLang
} from "../State"
import BrandHeader from "../Header"
import PlatformDropdownMenu from "../../platform/DropdownMenu"
import LangDropdownMenu from "../../language/DropdownMenu"
import { GameProviderDropdownMenu } from "../../game/provider/DropdownMenu"
import { GameList } from "../../game/List"
import {
    useChangePlayerPassword,
    useLogout,
    usePlayerInfo,
    usePlayerToken,
    useUpdatePlayerInfo
} from "../../client/brand/player"
import { useForm } from "react-hook-form"
import { ChangePasswordDialog } from "./ChangePasswordDialog"
import { EditPlayerInfoDialog, ViewPlayerInfoDialog } from "./InfoDialog"
import { toValue } from "../../platform"
import { DepositDialog } from "./DepositDialog"
import { UploadDepositProofDialog } from "./UploadDepositProofDialog"
import { BankInfoDialog } from "./BankInfoDialog"
import { WithdrawDialog } from "./WithdrawDialog"
import { InitPayPasswordDialog } from "./InitPayPasswordDialog"
import { JoinPromotionDialog } from "./JoinPromotion"

export const PlayerPage = ({ brandToken, ...props }) => {
    let { path, url } = useRouteMatch()

    return (
        <Switch>
            <Route path={`${path}/:platform`}>
                <PageWithHeader header={<Header />} {...props}>
                    <Body brandToken={brandToken} />
                </PageWithHeader>
            </Route>
        </Switch>
    )
}

const Body = ({ brandToken }) => {
    let { brandCode, username, platform } = useParams()

    let currency = useCurrency()
    let language = useLang()

    let [gameProviderCode, setGameProviderCode] = useState()

    return (
        <Stack padding="major-1" spacing="major-1">
            <Set>
                <Text use="strong">{platform}</Text>
                <PlayerDropdownMenu
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                />

                <GameProviderDropdownMenu
                    brandCode={brandCode}
                    currency={currency}
                    brandToken={brandToken}
                    username={username}
                    platform={platform}
                    language={language}
                    value={gameProviderCode}
                    onChange={setGameProviderCode}
                />
            </Set>
            <Box>
                <GameList
                    brandCode={brandCode}
                    username={username}
                    currency={currency}
                    platform={platform}
                    language={language}
                    gameProviderCode={gameProviderCode}
                />
            </Box>
        </Stack>
    )
}

const PlayerDropdownMenu = ({ brandCode, currency, username, platform }) => {
    let platformtype = toValue(platform)
    let { token: playerToken, error: playerTokenError } = usePlayerToken({
        brandCode,
        currency,
        username,
        platformtype
    })

    let [token, setLogoutToken] = useState()
    let { result, error, isLoading, ...rest } = useLogout(
        { brandCode, currency, username, platformtype },
        token
    )
    let history = useHistory()
    useEffect(() => {
        if (result) {
            history.push(`/${brandCode}/${currency}`)
        }
    }, [result])

    useEffect(() => {
        if (playerTokenError) {
            history.push(`/${brandCode}/${currency}`)
        }
    }, [playerTokenError])

    const depositDialog = Modal.useState()

    const [depositFormId, setDepositFormId] = useState()

    const uploadDepositProofDialog = Modal.useState()
    let showUploadDepositProofMenu = !!depositFormId

    const [depositInfo, setDepositInfo] = useState()
    const depositInfoDialog = Modal.useState()

    const withdrawDialog = Modal.useState()

    const bankInfoDialog = Modal.useState()

    const initPayPasswordDialog = Modal.useState()
    const changePayPasswordDialog = Modal.useState()

    const joinPromotionDialog = Modal.useState()

    const viewInfoDialog = Modal.useState()
    const editInfoDialog = Modal.useState()
    const changePasswordDialog = Modal.useState()

    return (
        <>
            <DropdownMenu
                menu={
                    <PlayerMenu
                        depositDialog={depositDialog}
                        showUploadDepositProofMenu={showUploadDepositProofMenu}
                        uploadDepositProofDialog={uploadDepositProofDialog}
                        withdrawDialog={withdrawDialog}
                        bankInfoDialog={bankInfoDialog}
                        initPayPasswordDialog={initPayPasswordDialog}
                        changePayPasswordDialog={changePayPasswordDialog}
                        playerInfoDialog={viewInfoDialog}
                        joinPromotionDialog={joinPromotionDialog}
                        editPlayerInfoDialog={editInfoDialog}
                        editPasswordDialog={changePasswordDialog}
                        logout={() => setLogoutToken(playerToken)}
                    />
                }
            >
                <Button iconAfter="chevron-down" isLoading={isLoading}>
                    {username}
                </Button>
            </DropdownMenu>
            {depositDialog.visible && (
                <DepositDialog
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    onSubmitted={(payment, value) => {
                        if (payment.paymentcode.toLowerCase() === "manual") {
                            setDepositFormId(value)

                            if (value) {
                                uploadDepositProofDialog.show()
                            }
                        } else {
                            setDepositInfo(value)

                            if (value) {
                                depositInfoDialog.show()
                            }
                        }
                        console.log("payment", payment, "value", value)
                    }}
                    dialog={depositDialog}
                />
            )}
            {uploadDepositProofDialog.visible && (
                <UploadDepositProofDialog
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    depositFormId={depositFormId}
                    dialog={uploadDepositProofDialog}
                />
            )}
            {depositInfo?.paymenturl && depositInfoDialog.visible && (
                <Modal.Dialog
                    title="Deposit Url"
                    showCloseButton
                    hideOnClickOutside={false}
                    {...depositInfoDialog}
                >
                    <_Link href={depositInfo.paymenturl} target="_blank">
                        {depositInfo?.paymenturl ?? ""}
                    </_Link>
                </Modal.Dialog>
            )}
            {withdrawDialog.visible && (
                <WithdrawDialog
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    dialog={withdrawDialog}
                />
            )}
            {bankInfoDialog.visible && (
                <BankInfoDialog
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    dialog={bankInfoDialog}
                />
            )}
            {initPayPasswordDialog.visible && (
                <InitPayPasswordDialog
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    dialog={initPayPasswordDialog}
                />
            )}
            {joinPromotionDialog.visible && (
                <JoinPromotionDialog
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    dialog={joinPromotionDialog}
                />
            )}
            {viewInfoDialog.visible && (
                <ViewPlayerInfoDialog
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    dialog={viewInfoDialog}
                />
            )}
            {editInfoDialog.visible && (
                <EditPlayerInfoDialog
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    dialog={editInfoDialog}
                />
            )}
            {changePasswordDialog.visible && (
                <ChangePasswordDialog
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    platform={platform}
                    dialog={changePasswordDialog}
                />
            )}
        </>
    )
}

const PlayerMenu = ({
    depositDialog,
    showUploadDepositProofMenu,
    uploadDepositProofDialog,
    withdrawDialog,
    joinPromotionDialog,
    bankInfoDialog,
    initPayPasswordDialog,
    changePayPasswordDialog,
    playerInfoDialog,
    editPlayerInfoDialog,
    editPasswordDialog,
    logout
}) => {
    return (
        <>
            <DropdownMenu.Item onClick={depositDialog.show}>
                Deposit
            </DropdownMenu.Item>
            {showUploadDepositProofMenu && (
                <DropdownMenu.Item onClick={uploadDepositProofDialog.show}>
                    Upload Deposit Proof
                </DropdownMenu.Item>
            )}
            <DropdownMenu.Item onClick={bankInfoDialog.show}>
                Bank List
            </DropdownMenu.Item>
            <DropdownMenu.Item onClick={initPayPasswordDialog.show}>
                Init Pay Password
            </DropdownMenu.Item>
            {/* <DropdownMenu.Item onClick={changePayPasswordDialog.show}>
                Change Pay Password
            </DropdownMenu.Item> */}
            <DropdownMenu.Item onClick={withdrawDialog.show}>
                Withdraw
            </DropdownMenu.Item>
            <DropdownMenu.Item onClick={joinPromotionDialog.show}>
                Join Promotion
            </DropdownMenu.Item>
            <DropdownMenu.Item onClick={playerInfoDialog.show}>
                View Info
            </DropdownMenu.Item>
            <DropdownMenu.Item onClick={editPlayerInfoDialog.show}>
                Edit Info
            </DropdownMenu.Item>
            <DropdownMenu.Item onClick={editPasswordDialog.show}>
                Change Password
            </DropdownMenu.Item>
            <DropdownMenu.Item onClick={logout}>Logout</DropdownMenu.Item>
        </>
    )
}

const Header = () => {
    let lang = useLang()
    let setLang = useSetLang()
    return (
        <BrandHeader
            navItems={
                <>
                    <TopNav.Item>
                        <LangDropdownMenu value={lang} onChange={setLang} />
                    </TopNav.Item>
                </>
            }
        />
    )
}

export default PlayerPage
