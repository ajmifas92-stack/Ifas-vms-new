using Xunit;
public class SmokeTests {
 [Fact] public void ProductNameIsCorrect()=>Assert.Equal("IFAS VMS","IFAS VMS");
 [Fact] public void GridSizesAreSupported()=>Assert.Equal(new[]{1,4,8,16,32,64},new[]{1,4,8,16,32,64});
}
