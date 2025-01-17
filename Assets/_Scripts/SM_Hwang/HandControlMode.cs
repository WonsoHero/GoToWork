//기본 조작 모드
public enum HandControlMode
{
    //조작 X : 걸어다닐 때
    None,
    //이동 모드
    Move,
    //회전 모드
    Rotate,
}
//이동 축 정의
public enum HandMoveAxis
{
    //상하좌우 이동
    All,
    //수직 이동
    Vertical,
    //수평 이동
    Horizontal,
}
//손 힘주기 방식 정의
public enum HandReverse
{
    //일반 : 왼손을 움직이며 왼손에 힘을 줌
    None,
    //역 : 왼손을 움직이며 오른손에 힘을 줌
    Reverse
}